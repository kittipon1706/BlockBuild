using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
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

        components["minecraft:collision_box"] = (data.collision == "false" || isDisabledBox) ? false :
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

        components["minecraft:geometry"] = new
        {
            identifier = "geometry." + data.namespaceId + "." + Path.GetFileNameWithoutExtension(data.geomerty).Replace(".geo","")
        };

        components["minecraft:material_instances"] = new Dictionary<string, object>
    {
        {
            "*", new {
                texture = data.texture,
                render_method = data.render_method
            }
        }
    };

        components["minecraft:destructible_by_mining"] = new
        {
            seconds_to_destroy = data.destroy_time.ToString(),
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
        var traits = new Dictionary<string, object>();

        switch (data.rotationType)
        {
            case "Cardinal":
                traits["minecraft:placement_direction"] = new
                {
                    enabled_states = new[] { "minecraft:cardinal_direction" },
                    y_rotation_offset = 180
                };
                permutations.AddRange(new[] {
                new { condition = "q.block_state('minecraft:cardinal_direction') == 'north'", components = new { minecraft__transformation = new { rotation = new float[] { 0, 0, 0 } } } },
                new { condition = "q.block_state('minecraft:cardinal_direction') == 'east'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, -90, 0 } } } },
                new { condition = "q.block_state('minecraft:cardinal_direction') == 'south'", components = new { minecraft__transformation = new { rotation = new float[] { 0, 180, 0 } } } },
                new { condition = "q.block_state('minecraft:cardinal_direction') == 'west'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, 90, 0 } } } }
            });
                break;

            case "Facing":
                traits["minecraft:placement_direction"] = new
                {
                    enabled_states = new[] { "minecraft:facing_direction" },
                    y_rotation_offset = 180
                };
                permutations.AddRange(new[] {
                new { condition = "q.block_state('minecraft:facing_direction') == 'up'",    components = new { minecraft__transformation = new { rotation = new float[] { -90, 0, 0 } } } },
                new { condition = "q.block_state('minecraft:facing_direction') == 'down'",  components = new { minecraft__transformation = new { rotation = new float[] { 90, 0, 0 } } } },
                new { condition = "q.block_state('minecraft:facing_direction') == 'north'", components = new { minecraft__transformation = new { rotation = new float[] { 0, 0, 0 } } } },
                new { condition = "q.block_state('minecraft:facing_direction') == 'east'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, -90, 0 } } } },
                new { condition = "q.block_state('minecraft:facing_direction') == 'south'", components = new { minecraft__transformation = new { rotation = new float[] { 0, 180, 0 } } } },
                new { condition = "q.block_state('minecraft:facing_direction') == 'west'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, 90, 0 } } } }
            });
                break;

            case "Cardinal Facing":
                traits["minecraft:placement_direction"] = new
                {
                    enabled_states = new[] { "minecraft:cardinal_direction", "minecraft:facing_direction" }
                };
                permutations.AddRange(new[] {
                new { condition = "query.block_state('minecraft:facing_direction') == 'north'", components = new { minecraft__transformation = new { rotation = new float[] { 0, 180, 0 } } } },
                new { condition = "query.block_state('minecraft:facing_direction') == 'east'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, 90, 0 } } } },
                new { condition = "query.block_state('minecraft:facing_direction') == 'south'", components = new { minecraft__transformation = new { rotation = new float[] { 0, 0, 0 } } } },
                new { condition = "query.block_state('minecraft:facing_direction') == 'west'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, -90, 0 } } } },
                new { condition = "query.block_state('minecraft:facing_direction') == 'down' && query.block_state('minecraft:cardinal_direction') == 'north'",  components = new { minecraft__transformation = new { rotation = new float[] { 90, 180, 0 } } } },
                new { condition = "query.block_state('minecraft:facing_direction') == 'down' && query.block_state('minecraft:cardinal_direction') == 'south'",  components = new { minecraft__transformation = new { rotation = new float[] { 90, 0, 0 } } } },
                new { condition = "query.block_state('minecraft:facing_direction') == 'down' && query.block_state('minecraft:cardinal_direction') == 'west'",  components = new { minecraft__transformation = new { rotation = new float[] { 90, -90, 0 } } } },
                new { condition = "query.block_state('minecraft:facing_direction') == 'down' && query.block_state('minecraft:cardinal_direction') == 'east'",  components = new { minecraft__transformation = new { rotation = new float[] { 90, 90, 0 } } } },
                   new { condition = "query.block_state('minecraft:facing_direction') == 'up' && query.block_state('minecraft:cardinal_direction') == 'north'",  components = new { minecraft__transformation = new { rotation = new float[] { -90, 180, 0 } } } },
                new { condition = "query.block_state('minecraft:facing_direction') == 'up' && query.block_state('minecraft:cardinal_direction') == 'south'",  components = new { minecraft__transformation = new { rotation = new float[] { -90, 0, 0 } } } },
                new { condition = "query.block_state('minecraft:facing_direction') == 'up' && query.block_state('minecraft:cardinal_direction') == 'west'",  components = new { minecraft__transformation = new { rotation = new float[] { -90, -90, 0 } } } },
                new { condition = "query.block_state('minecraft:facing_direction') == 'up' && query.block_state('minecraft:cardinal_direction') == 'east'",  components = new { minecraft__transformation = new { rotation = new float[] { -90, 90, 0 } } } }
            });
                break;

            case "Cardinal Block Face":
                traits["minecraft:placement_direction"] = new
                {
                    enabled_states = new[] { "minecraft:cardinal_direction" }
                };
                traits["minecraft:placement_direction"] = new
                {
                    enabled_states = new[] { "minecraft:block_face" }
                };
                permutations.AddRange(new[] {
                new { condition = "query.block_state('minecraft:block_face') == 'north'", components = new { minecraft__transformation = new { rotation = new float[] { 0, 180, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'east'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, 90, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'south'", components = new { minecraft__transformation = new { rotation = new float[] { 0, 0, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'west'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, -90, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'down' && q.block_state('minecraft:cardinal_direction') == 'nort'",  components = new { minecraft__transformation = new { rotation = new float[] { 90, 180, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'down' && q.block_state('minecraft:cardinal_direction') == 'south'",  components = new { minecraft__transformation = new { rotation = new float[] { 90, 0, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'down' && q.block_state('minecraft:cardinal_direction') == 'west'",  components = new { minecraft__transformation = new { rotation = new float[] { 90, -90, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'down' && q.block_state('minecraft:cardinal_direction') == 'east'",  components = new { minecraft__transformation = new { rotation = new float[] { 90, 90, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'up' && q.block_state('minecraft:cardinal_direction') == 'nort'",  components = new { minecraft__transformation = new { rotation = new float[] { -90, 180, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'up' && q.block_state('minecraft:cardinal_direction') == 'south'",  components = new { minecraft__transformation = new { rotation = new float[] { -90, 0, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'up' && q.block_state('minecraft:cardinal_direction') == 'west'",  components = new { minecraft__transformation = new { rotation = new float[] { -90, -90, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'up' && q.block_state('minecraft:cardinal_direction') == 'east'",  components = new { minecraft__transformation = new { rotation = new float[] { -90, 90, 0 } } } }
            });
                break;

            case "Block Face":
                traits["minecraft:placement_direction"] = new
                {
                    enabled_states = new[] { "minecraft:block_face" }
                };
                permutations.AddRange(new[] {
                new { condition = "query.block_state('minecraft:block_face') == 'up'",    components = new { minecraft__transformation = new { rotation = new float[] { 90, 0, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'down'",  components = new { minecraft__transformation = new { rotation = new float[] { -90, 0, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'north'", components = new { minecraft__transformation = new { rotation = new float[] { 0, 0, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'east'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, -90, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'south'", components = new { minecraft__transformation = new { rotation = new float[] { 0, 180, 0 } } } },
                new { condition = "query.block_state('minecraft:block_face') == 'west'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, 90, 0 } } } }
            });
                break;

            case "Cardinal Vertical Half":
                traits["minecraft:placement_direction"] = new
                {
                    enabled_states = new[] { "minecraft:cardinal_direction" },
                    y_rotation_offset = 180
                };
                traits["minecraft:placement_direction"] = new
                {
                    enabled_states = new[] { "minecraft:vertical_half" }
                };
                permutations.AddRange(new[] {
                new { condition = "q.block_state('minecraft:cardinal_direction') == 'north'", components = new { minecraft__transformation = new { rotation = new float[] { 0, 0, 0 } } } },
                new { condition = "q.block_state('minecraft:cardinal_direction') == 'east'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, -90, 0 } } } },
                new { condition = "q.block_state('minecraft:cardinal_direction') == 'south'", components = new { minecraft__transformation = new { rotation = new float[] { 0, 180, 0 } } } },
                new { condition = "q.block_state('minecraft:cardinal_direction') == 'west'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, 90, 0 } } } },
                new { condition = "q.block_state('minecraft:cardinal_direction') == 'north' && q.block_state('minecraft:vertical_half') == 'top'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, 0, 180 } } } },
                new { condition = "q.block_state('minecraft:cardinal_direction') == 'west' && q.block_state('minecraft:vertical_half') == 'top'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, -90, 180 } } } },
                new { condition = "q.block_state('minecraft:cardinal_direction') == 'south' && q.block_state('minecraft:vertical_half') == 'top'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, 180, 180 } } } },
                new { condition = "q.block_state('minecraft:cardinal_direction') == 'east' && q.block_state('minecraft:vertical_half') == 'top'",  components = new { minecraft__transformation = new { rotation = new float[] { 0, 90, 180 } } } },
                });
                break;

            case "Vertical Half":
                traits["minecraft:placement_direction"] = new
                {
                    enabled_states = new[] { "minecraft:vertical_half" }
                };
                permutations.Add(new { condition = "query.block_state('minecraft:vertical_half') == 'top'", components = new { minecraft__transformation = new { translation = new float[] { 0, 0.5f, 0 } } } });
                break;
        }

        var description = new Dictionary<string, object>
    {
        { "identifier", data.Identifier },
        { "menu_category", new { category = "construction" } }
    };

        if (traits.Count > 0)
        {
            description["traits"] = traits;
        }

        var blockJson = new Dictionary<string, object>
    {
        { "format_version", data.format_Version },
        {
            "minecraft:block", new Dictionary<string, object>
            {
                { "description", description },
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

    [System.Serializable]
    public class Translation
    {
        public float[] translation;
    }
}
