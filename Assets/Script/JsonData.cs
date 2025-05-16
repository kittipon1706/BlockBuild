using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Data;

public class JsonData : MonoBehaviour
{
    public static string GenerateBlockJson(BlockData data)
    {
        var components = new Dictionary<string, object>();

        bool isDefaultBox = data.selectionBox_origin == new Vector3(-8, 0, -8) &&
                            data.selectionBox_size == new Vector3(16, 16, 16);

        bool isDisabledBox = data.selectionBox_size == Vector3.zero;

        // กำหนด selection_box
        components["minecraft:selection_box"] = isDefaultBox ? true :
            isDisabledBox ? false : new
            {
                origin = new float[] {
                data.selectionBox_origin.x,
                data.selectionBox_origin.y,
                data.selectionBox_origin.z
                },
                size = new float[] {
                data.selectionBox_size.x,
                data.selectionBox_size.y,
                data.selectionBox_size.z
                }
            };

        // กำหนด collision_box
        components["minecraft:collision_box"] = (!data.collision || isDisabledBox) ? false :
            isDefaultBox ? true : new
            {
                origin = new float[] {
                data.selectionBox_origin.x,
                data.selectionBox_origin.y,
                data.selectionBox_origin.z
                },
                size = new float[] {
                data.selectionBox_size.x,
                data.selectionBox_size.y,
                data.selectionBox_size.z
                }
            };

        // minecraft:geometry
        components["minecraft:geometry"] = new
        {
            identifier = "geometry." + data.namespaceId + "." + Path.GetFileNameWithoutExtension(data.geomerty).Replace(".geo", "")
        };

        // minecraft:material_instances
        components["minecraft:material_instances"] = new Dictionary<string, object>
    {
        {
            "*", new {
                texture =  data.namespaceId + ":" + Path.GetFileNameWithoutExtension(data.texture),
                render_method = data.render_method
            }
        }
    };

        components["minecraft:destructible_by_mining"] = new
        {
            seconds_to_destroy = data.destroy_time,
            item_specific_speeds = new[]
            {
            new
            {
                item = new { tags = "q.any_tag('minecraft:is_pickaxe')" },
                destroy_speed = 0.5f
            }
        }
        };

        components["tag:minecraft:is_pickaxe_item_destructible"] = new { };

        var permutations = new List<object>();

        switch (data.rotationType)
        {
            case "Default":
                // ไม่มีการหมุนพิเศษ
                break;

            case "Cardinal":
                // หมุนตามแกน Y 4 ทิศ (facing)
                permutations.AddRange(new[]
                {
                new { condition = "q.block_state.facing == north", components = new { minecraft__transformation = new { rotation = new float[] {0, 0, 0} } } },
                new { condition = "q.block_state.facing == east",  components = new { minecraft__transformation = new { rotation = new float[] {0, 90, 0} } } },
                new { condition = "q.block_state.facing == south", components = new { minecraft__transformation = new { rotation = new float[] {0, 180, 0} } } },
                new { condition = "q.block_state.facing == west",  components = new { minecraft__transformation = new { rotation = new float[] {0, 270, 0} } } }
            });
                break;

            case "Cardinal Facing":
                // หมุน 6 ทิศ (facing) และอาจมีการหมุนเพิ่มเติมตามตำแหน่ง (ส่วนขยายในอนาคต)
                permutations.AddRange(new[]
                {
                new { condition = "q.block_state.facing == north", components = new { minecraft__transformation = new { rotation = new float[] {0, 0, 0} } } },
                new { condition = "q.block_state.facing == east",  components = new { minecraft__transformation = new { rotation = new float[] {0, 90, 0} } } },
                new { condition = "q.block_state.facing == south", components = new { minecraft__transformation = new { rotation = new float[] {0, 180, 0} } } },
                new { condition = "q.block_state.facing == west",  components = new { minecraft__transformation = new { rotation = new float[] {0, 270, 0} } } }
            });
                // TODO: เพิ่มการหมุนตามตำแหน่ง block face ในอนาคต
                break;

            case "Cardinal Block Face":
                // วางบนพื้นผิวบล็อก (block_face) 4 ทิศ
                permutations.AddRange(new[]
                {
                new { condition = "q.block_state.block_face == north", components = new { minecraft__transformation = new { rotation = new float[] {0, 0, 0} } } },
                new { condition = "q.block_state.block_face == east",  components = new { minecraft__transformation = new { rotation = new float[] {0, 90, 0} } } },
                new { condition = "q.block_state.block_face == south", components = new { minecraft__transformation = new { rotation = new float[] {0, 180, 0} } } },
                new { condition = "q.block_state.block_face == west",  components = new { minecraft__transformation = new { rotation = new float[] {0, 270, 0} } } }
            });
                break;

            case "Cardinal Vertical Half":
                // วางบนพื้นผิวบล็อก (block_face) 4 ทิศ และแบ่งช่วงบน/ล่าง (vertical_half)
                permutations.AddRange(new[]
                {
                new { condition = "q.block_state.vertical_half == top", components = new { minecraft__transformation = new { rotation = new float[] {180, 0, 0} } } }
            });
                break;

            case "Facing":
                // หมุน 6 ทิศ (facing)
                permutations.AddRange(new[]
                {
                new { condition = "q.block_state.facing == north", components = new { minecraft__transformation = new { rotation = new float[] {0, 0, 0} } } },
                new { condition = "q.block_state.facing == east",  components = new { minecraft__transformation = new { rotation = new float[] {0, 90, 0} } } },
                new { condition = "q.block_state.facing == south", components = new { minecraft__transformation = new { rotation = new float[] {0, 180, 0} } } },
                new { condition = "q.block_state.facing == west",  components = new { minecraft__transformation = new { rotation = new float[] {0, 270, 0} } } },
                new { condition = "q.block_state.facing == up",    components = new { minecraft__transformation = new { rotation = new float[] {270, 0, 0} } } },
                new { condition = "q.block_state.facing == down",  components = new { minecraft__transformation = new { rotation = new float[] {90, 0, 0} } } }
            });
                break;

            case "Block Face":
                // วางบนพื้นผิวบล็อก 6 ทิศ (block_face)
                permutations.AddRange(new[]
                {
                new { condition = "q.block_state.block_face == north", components = new { minecraft__transformation = new { rotation = new float[] {0, 0, 0} } } },
                new { condition = "q.block_state.block_face == east",  components = new { minecraft__transformation = new { rotation = new float[] {0, 90, 0} } } },
                new { condition = "q.block_state.block_face == south", components = new { minecraft__transformation = new { rotation = new float[] {0, 180, 0} } } },
                new { condition = "q.block_state.block_face == west",  components = new { minecraft__transformation = new { rotation = new float[] {0, 270, 0} } } },
                new { condition = "q.block_state.block_face == up",    components = new { minecraft__transformation = new { rotation = new float[] {270, 0, 0} } } },
                new { condition = "q.block_state.block_face == down",  components = new { minecraft__transformation = new { rotation = new float[] {90, 0, 0} } } }
            });
                break;

            case "Vertical Half":
                // วางบนพื้นผิวบล็อก ช่วงบน/ล่าง (vertical_half)
                permutations.AddRange(new[]
                {
                new { condition = "q.block_state.vertical_half == top", components = new { minecraft__transformation = new { rotation = new float[] {180, 0, 0} } } }
            });
                break;
        }

        var blockJson = new Dictionary<string, object>
    {
        { "format_version", data.format_Version },
        {
            "minecraft:block", new Dictionary<string, object>
            {
                {
                    "description", new Dictionary<string, object>
                    {
                        { "identifier", data.Identifier },
                        { "menu_category", new { category = "construction" } }
                    }
                },
                { "components", components }
            }
        }
    };

        if (permutations.Count > 0)
        {
            ((Dictionary<string, object>)blockJson["minecraft:block"]).Add("permutations", permutations);
        }

        return JsonConvert.SerializeObject(blockJson, Formatting.Indented);
    }


    public static void SaveToFile(string outputPath, BlockData data)
    {
        string json = GenerateBlockJson(data);
        File.WriteAllText(outputPath, json);
    }


[System.Serializable]
    public class BlockFile
    {
        public string format_version;
        public MinecraftBlock minecraft__block;
    }

    [System.Serializable]
    public class MinecraftBlock
    {
        public BlockDescription description;
        public BlockComponents components;
        public BlockPermutation[] permutations;
    }

    [System.Serializable]
    public class BlockDescription
    {
        public string identifier;
        public MenuCategory menu_category;
        public Traits traits;
    }

    [System.Serializable]
    public class MenuCategory
    {
        public string category;
    }

    [System.Serializable]
    public class Traits
    {
        public PlacementDirection minecraft__placement_direction;
    }

    [System.Serializable]
    public class PlacementDirection
    {
        public string[] enabled_states;
    }

    [System.Serializable]
    public class BlockComponents
    {
        public Box minecraft__selection_box;
        public Box minecraft__collision_box;
        public Geometry minecraft__geometry;
        public MaterialInstances minecraft__material_instances;
        public bool minecraft__destructible_by_explosion;
        public bool minecraft__flammable;
        public DestructibleByMining minecraft__destructible_by_mining;
        public TagPickaxe tag__minecraft__is_pickaxe_item_destructible;
    }

    [System.Serializable]
    public class Box
    {
        public float[] origin;
        public float[] size;
    }

    [System.Serializable]
    public class Geometry
    {
        public string identifier;
    }

    [System.Serializable]
    public class MaterialInstances
    {
        public TextureInfo up;
        public TextureInfo down;
        public TextureInfo custom_sides;
        public string north;
        public string south;
        public string east;
        public string west;
    }

    [System.Serializable]
    public class TextureInfo
    {
        public string texture;
    }

    [System.Serializable]
    public class DestructibleByMining
    {
        public float seconds_to_destroy;
        public ItemSpecificSpeed[] item_specific_speeds;
    }

    [System.Serializable]
    public class ItemSpecificSpeed
    {
        public Item item;
        public float destroy_speed;
    }

    [System.Serializable]
    public class Item
    {
        public string tags;
    }

    [System.Serializable]
    public class TagPickaxe
    {
        // empty object → no fields
    }

    [System.Serializable]
    public class BlockPermutation
    {
        public string condition;
        public PermutationComponent components;
    }

    [System.Serializable]
    public class PermutationComponent
    {
        public Rotation minecraft__transformation;
    }

    [System.Serializable]
    public class Rotation
    {
        public float[] rotation;
    }

}
