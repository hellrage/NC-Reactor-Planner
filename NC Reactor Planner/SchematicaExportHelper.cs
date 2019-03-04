using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Text;
using System.Threading.Tasks;
using fNbt;

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
                        Block block = Reactor.BlockAt(new Point3D(x, y, z));
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
            NbtCompound activeCooler = new NbtCompound();
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
