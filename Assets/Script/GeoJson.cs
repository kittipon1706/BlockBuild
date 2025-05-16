using System.Collections.Generic;
using Newtonsoft.Json;

public class GeoJsonRoot
{
    [JsonProperty("format_version")]
    public string format_version { get; set; }

    [JsonProperty("minecraft:geometry")]
    public List<MinecraftGeometry> minecraft_geometry { get; set; }
}

public class MinecraftGeometry
{
    public Description description { get; set; }
    public List<Bone> bones { get; set; }
}

public class Description
{
    public string identifier { get; set; }
    public int texture_width { get; set; }
    public int texture_height { get; set; }
    public float visible_bounds_width { get; set; }
    public float visible_bounds_height { get; set; }
    public float[] visible_bounds_offset { get; set; }
}

public class Bone
{
    public string name { get; set; }
    public float[] pivot { get; set; }
    public List<Cube> cubes { get; set; }
}

public class Cube
{
    public float[] origin { get; set; }
    public float[] size { get; set; }
    public CubeUVMap uv { get; set; }
}

public class CubeUVMap
{
    public FaceUV north { get; set; }
    public FaceUV east { get; set; }
    public FaceUV south { get; set; }
    public FaceUV west { get; set; }
    public FaceUV up { get; set; }
    public FaceUV down { get; set; }
}

public class FaceUV
{
    public float[] uv { get; set; }

    [JsonProperty("uv_size")]
    public float[] uv_size { get; set; }

    [JsonProperty("uv_rotation")]
    public int? uv_rotation { get; set; }
}
