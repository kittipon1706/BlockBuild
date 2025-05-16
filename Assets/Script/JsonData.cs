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
        bool isDefaultBox =
            data.selectionBox_origin == new Vector3(-8, 0, -8) &&
            data.selectionBox_size == new Vector3(16, 16, 16);

        bool isEmptyBox = data.selectionBox_size == Vector3.zero;

        var components = new Dictionary<string, object>();

        // minecraft:selection_box
        components["minecraft:selection_box"] = isEmptyBox
            ? false
            : (isDefaultBox ? true : new
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
            });

        // minecraft:collision_box
        if (data.collision && !isEmptyBox)
        {
            components["minecraft:collision_box"] = isDefaultBox
                ? true
                : new
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
        }
        else
        {
            components["minecraft:collision_box"] = false;
        }

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

        // minecraft:destructible_by_mining
        components["minecraft:destructible_by_mining"] = new
        {
            seconds_to_destroy = data.destroy_time,
            item_specific_speeds = new[] {
            new {
                item = new {
                    tags = "q.any_tag('minecraft:is_pickaxe') "
                },
                destroy_speed = 0.5f
            }
        }
        };

        // tag:minecraft:is_pickaxe_item_destructible
        components["tag:minecraft:is_pickaxe_item_destructible"] = new { };

        // Build block JSON
        var blockJson = new
        {
            format_version = data.format_Version,
            minecraft__block = new
            {
                description = new
                {
                    identifier = data.Identifier,
                    menu_category = new
                    {
                        category = "construction"
                    }
                },
                components = components
            }
        };

        // Final root object
        var root = new Dictionary<string, object>
    {
        { "format_version", blockJson.format_version },
        { "minecraft:block", blockJson.minecraft__block }
    };

        return JsonConvert.SerializeObject(root, Formatting.Indented);
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
