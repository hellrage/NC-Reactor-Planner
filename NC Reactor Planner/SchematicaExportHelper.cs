using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using fNbt;
using System.Numerics;

namespace NC_Reactor_Planner
{
    public static class SchematicaExportHelper
    {
        private static Dictionary<string, int> BlockMetaLookup = new Dictionary<string, int>();
        private static Dictionary<BlockTypes, int> BlockIDLookup = new Dictionary<BlockTypes, int>();
        private static Dictionary<BlockTypes, string> SchematicaMappingLookup = new Dictionary<BlockTypes, string>();

        static SchematicaExportHelper()
        {
            BlockMetaLookup.Add(BlockTypes.Air.ToString(), 0);
            BlockMetaLookup.Add(BlockTypes.FuelCell.ToString(), 0);

            BlockMetaLookup.Add(ModeratorTypes.Graphite.ToString(), 8);
            BlockMetaLookup.Add(ModeratorTypes.Beryllium.ToString(), 9);
            BlockMetaLookup.Add(CoolerTypes.Copper.ToString(), 13);
            BlockMetaLookup.Add(CoolerTypes.Cryotheum.ToString(), 10);
            BlockMetaLookup.Add(CoolerTypes.Diamond.ToString(), 7);
            BlockMetaLookup.Add(CoolerTypes.Emerald.ToString(), 12);
            BlockMetaLookup.Add(CoolerTypes.Enderium.ToString(), 9);
            BlockMetaLookup.Add(CoolerTypes.Glowstone.ToString(), 5);
            BlockMetaLookup.Add(CoolerTypes.Gold.ToString(), 4);
            BlockMetaLookup.Add(CoolerTypes.Helium.ToString(), 8);
            BlockMetaLookup.Add(CoolerTypes.Iron.ToString(), 11);
            BlockMetaLookup.Add(CoolerTypes.Lapis.ToString(), 6);
            BlockMetaLookup.Add(CoolerTypes.Magnesium.ToString(), 15);
            BlockMetaLookup.Add(CoolerTypes.Quartz.ToString(), 3);
            BlockMetaLookup.Add(CoolerTypes.Redstone.ToString(), 2);
            BlockMetaLookup.Add(CoolerTypes.Tin.ToString(), 14);
            BlockMetaLookup.Add(CoolerTypes.Water.ToString(), 1);

            BlockIDLookup.Add(BlockTypes.Air, 0);
            BlockIDLookup.Add(BlockTypes.FuelCell, 258);
            BlockIDLookup.Add(BlockTypes.Cooler, 259);
            BlockIDLookup.Add(BlockTypes.Moderator, 254);

            SchematicaMappingLookup.Add(BlockTypes.Cooler, "nuclearcraft:cooler");
            SchematicaMappingLookup.Add(BlockTypes.FuelCell, "nuclearcraft:cell_block");
            SchematicaMappingLookup.Add(BlockTypes.Moderator, "nuclearcraft:ingot_block");
        }

        public static NbtCompound ExportReactor()
        {
            NbtCompound reactor = new NbtCompound("Schematic");
            bool coolerIsActive = false;
            int volume = (int)(Reactor.interiorDims.X * Reactor.interiorDims.Y * Reactor.interiorDims.Z);
            byte[] blocks = new byte[volume];
            byte[] data = new byte[volume];
            bool extra = false;
            byte[] extraBlocks = new byte[volume];
            byte[] extraBlocksNibble = new byte[(int)Math.Ceiling(volume / 2.0)];
            NbtList tileEntities = new NbtList("TileEntities", NbtTagType.Compound);
            Dictionary<string, short> mappings = new Dictionary<string, short>();

            for (int y = 1; y <= Reactor.interiorDims.Y; y++)
                for(int z = 1; z<= Reactor.interiorDims.Z;z++)
                    for(int x = 1; x <= Reactor.interiorDims.X;x++)
                    {
                        coolerIsActive = false;
                        Block block = Reactor.BlockAt(new Vector3(x, y, z));
                        int index = (int)((x-1) + ((y-1) * Reactor.interiorDims.Z + (z-1)) * Reactor.interiorDims.X);
                        blocks[index] = (byte)BlockIDLookup[block.BlockType];
                        if (block.BlockType == BlockTypes.FuelCell | block.BlockType == BlockTypes.Air)
                            data[index] = 0;
                        else if (block is Cooler cooler)
                        {
                            if (cooler.Active)
                            {
                                coolerIsActive = true;
                                blocks[index] = 340-256;
                                data[index] = 0;
                                tileEntities.Add(CreateActiveCooler(x - 1, y - 1, z - 1));
                            }
                            else
                            {
                                data[index] = (byte)BlockMetaLookup[cooler.CoolerType.ToString()];
                            }
                        }
                        else if (block.BlockType == BlockTypes.Moderator)
                            data[index] = (byte)BlockMetaLookup[((Moderator)block).ModeratorType.ToString()];

                        if (coolerIsActive)
                        {
                            extraBlocks[index] = (byte)(340 >> 8);
                            extra = true;
                        }
                        else
                            if ((extraBlocks[index] = (byte)(BlockIDLookup[block.BlockType] >> 8)) > 0)

                            if (block.BlockType == BlockTypes.Air)
                                continue;
                        if(coolerIsActive)
                            if (!mappings.ContainsKey("nuclearcraft:active_cooler"))
                                mappings.Add("nuclearcraft:active_cooler", 340);
                        else
                            if (!mappings.ContainsKey(SchematicaMappingLookup[block.BlockType]))
                                mappings.Add(SchematicaMappingLookup[block.BlockType], (short)BlockIDLookup[block.BlockType]);
                    }

            for (int i = 0; i < extraBlocksNibble.Length; i++)
            {
                if (i * 2 + 1 < extraBlocks.Length)
                {
                    extraBlocksNibble[i] = (byte)((extraBlocks[i * 2 + 0] << 4) | extraBlocks[i * 2 + 1]);
                }
                else
                {
                    extraBlocksNibble[i] = (byte)(extraBlocks[i * 2 + 0] << 4);
                }
            }

            reactor.Add(new NbtByteArray("Blocks", blocks));
            reactor.Add(new NbtShort("Length", (short)Reactor.interiorDims.Z));
            reactor.Add(new NbtString("Materials", "Alpha"));
            reactor.Add(new NbtShort("Height", (short)Reactor.interiorDims.Y));
            reactor.Add(new NbtByteArray("Data", data));
            reactor.Add(SetIcon());
            NbtCompound mappingsC = new NbtCompound("SchematicaMapping");
            foreach (KeyValuePair<string, short> kvp in mappings)
                mappingsC.Add(new NbtShort(kvp.Key, kvp.Value));
            reactor.Add(mappingsC);
            reactor.Add(new NbtShort("Width", (short)Reactor.interiorDims.X));
            if(extra)
                reactor.Add(new NbtByteArray("AddBlocks", extraBlocksNibble));
            reactor.Add(tileEntities);
            reactor.Add(new NbtList("Entities", NbtTagType.Compound));



            return reactor;
        }

        public static NbtCompound ExportAsStructure()
        {
            NbtCompound reactor = new NbtCompound("ReactorStructure");
            int volume = (int)(Reactor.interiorDims.X * Reactor.interiorDims.Y * Reactor.interiorDims.Z);
            List<string> listPalette = new List<string>();
            NbtList palette = new NbtList("palette", NbtTagType.Compound);
            NbtList blocks = new NbtList("blocks",NbtTagType.Compound);
            NbtString author = new NbtString("author", "Hellrage");
            NbtCompound forgeDataVersion = new NbtCompound("ForgeDataVersion", new List<NbtInt>{ new NbtInt("minecraft", 1343) });
            NbtInt dataVersion = new NbtInt("DataVersion", 1342);

            for (int y = 1; y <= Reactor.interiorDims.Y; y++)
            {
                for (int z = 1; z <= Reactor.interiorDims.Z; z++)
                {
                    for (int x = 1; x <= Reactor.interiorDims.X; x++)
                    {
                        Block block = Reactor.BlockAt(new Vector3(x, y, z));
                        NbtCompound palettenbt = GetNbtCompound(block);
                        if (!listPalette.Contains(block.DisplayName))
                        {
                            listPalette.Add(block.DisplayName);
                            palette.Add(palettenbt);
                        }
                        NbtCompound blocknbt = new NbtCompound();
                        if (block.DisplayName.Contains("Active"))
                            blocknbt.Add(CreateActiveCooler(x - 1, y - 1, z - 1));
                        blocknbt.Add(new NbtList("pos", new List<NbtInt> { new NbtInt(x - 1), new NbtInt(y - 1), new NbtInt(z - 1) }));
                        blocknbt.Add(new NbtInt("state", listPalette.IndexOf(block.DisplayName)));
                        blocks.Add(blocknbt);
                        }
                }
            }

            reactor.Add(new NbtList("size", new List<NbtInt>{ new NbtInt((int)Reactor.interiorDims.X), new NbtInt((int)Reactor.interiorDims.Y), new NbtInt((int)Reactor.interiorDims.Z) }));
            reactor.Add(new NbtList("entities", new List<NbtInt>(), NbtTagType.Compound));
            reactor.Add(blocks);
            reactor.Add(author);
            reactor.Add(palette);
            reactor.Add(forgeDataVersion);
            reactor.Add(dataVersion);
            return reactor;
        }

        private static NbtCompound GetNbtCompound(Block block)
        {
            BlockTypes bt = block.BlockType;
            NbtCompound nbt = new NbtCompound();
            if (block.BlockType == BlockTypes.Air)
                nbt.Add(new NbtString("Name", "minecraft:air"));
            else if(bt == BlockTypes.FuelCell)
                nbt.Add(new NbtString("Name", "nuclearcraft:cell_block"));
            else if(bt == BlockTypes.Moderator)
            {
                nbt.Add(new NbtCompound("Properties", new List<NbtString> { new NbtString("type", ((Moderator)block).ModeratorType.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture)) }));
                nbt.Add(new NbtString("Name", "nuclearcraft:ingot_block"));
            }
            else if(bt== BlockTypes.Cooler)
            {
                if (((Cooler)block).Active)
                    nbt.Add(new NbtString("Name", "nuclearcraft:active_cooler"));
                else
                {
                    nbt.Add(new NbtString("Name", "nuclearcraft:cooler"));
                    nbt.Add(new NbtCompound("Properties", new List<NbtString> { new NbtString("type", ((Cooler)block).CoolerType.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture)) }));
                }
            }

            return nbt;
        }

        private static NbtCompound SetIcon()
        {
            NbtCompound icon = new NbtCompound("Icon");
            icon.Add(new NbtString("id", "nuclearcraft:rtg_plutonium"));
            icon.Add(new NbtByte("Count", 1));
            icon.Add(new NbtShort("Damage", 0));

            return icon;
        }

        private static NbtCompound CreateActiveCooler(int x, int y, int z)
        {
            NbtCompound activeCooler = new NbtCompound("nbt");
            activeCooler.Add(new NbtByte("isRedstonePowered", 0));
            activeCooler.Add(new NbtByte("emptyUnusable", 0));
            activeCooler.Add(new NbtByte("areTanksShared", 0));
            activeCooler.Add(new NbtByte("alternateComparator", 0));
            activeCooler.Add(new NbtString("fluidName0", "nullFluid"));
            activeCooler.Add(new NbtByte("isActive", 0));
            activeCooler.Add(new NbtInt("fluidAmount", 0));
            activeCooler.Add(new NbtInt("fluidConnections0", 0));
            activeCooler.Add(new NbtByte("voidExcessOutputs", 0));
            activeCooler.Add(new NbtInt("fluidConnections2", 0));
            activeCooler.Add(new NbtDouble("radiationLevel1", 0));
            activeCooler.Add(new NbtInt("fluidConnections1", 0));
            activeCooler.Add(new NbtInt("x", x));
            activeCooler.Add(new NbtInt("fluidConnections4", 0));
            activeCooler.Add(new NbtInt("y", y));
            activeCooler.Add(new NbtInt("fluidConnections3", 0));
            activeCooler.Add(new NbtInt("z", z));
            activeCooler.Add(new NbtString("id", "nuclearcraft:active_cooler"));
            activeCooler.Add(new NbtInt("fluidConnections5", 0));

            return activeCooler;
        }
    }
}
