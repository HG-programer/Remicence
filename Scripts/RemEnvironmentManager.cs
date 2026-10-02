using UnityEngine;

namespace Remniscence
{
    /// <summary>
    /// Controls the immersive 3D background environment for Rem.
    /// Can automatically generate a stylish anime study room (Mansion study, wood parquet, arched window,
    /// interactive tea set, bookshelf, desk, and ambient mana particles) if no pre-built scene is assigned.
    /// </summary>
    public class RemEnvironmentManager : MonoBehaviour
    {
        [Header("Room Dimensions")]
        [SerializeField] private float roomWidth = 7.0f;
        [SerializeField] private float roomLength = 7.0f;
        [SerializeField] private float roomHeight = 3.6f;

        [Header("Ambiance & Atmosphere")]
        [SerializeField] private bool autoBuildRoomIfEmpty = true;
        [SerializeField] private Color wallColor = new Color(0.92f, 0.90f, 0.86f);
        [SerializeField] private Color floorColor = new Color(0.24f, 0.16f, 0.10f); // Polished walnut parquet
        [SerializeField] private Color furnitureColor = new Color(0.32f, 0.20f, 0.12f); // Antique wood

        [Header("Lighting & Weather")]
        [SerializeField] private Light sunLight;
        [SerializeField] private Light deskLampLight;
        [SerializeField] private ParticleSystem manaEmbers;

        private GameObject generatedRoomContainer;

        void Awake()
        {
            if (autoBuildRoomIfEmpty && transform.childCount == 0)
            {
                BuildProceduralStudyRoom();
            }
        }

        /// <summary>
        /// Procedurally construct an elegant study room for Rem
        /// </summary>
        public void BuildProceduralStudyRoom()
        {
            if (generatedRoomContainer != null) DestroyImmediate(generatedRoomContainer);

            generatedRoomContainer = new GameObject("Rem_MansionStudy_Environment");
            generatedRoomContainer.transform.SetParent(transform, false);

            Material wallMat = CreateSimpleMaterial("WallMat", wallColor, 0.85f);
            Material floorMat = CreateSimpleMaterial("FloorMat", floorColor, 0.25f);
            Material woodMat = CreateSimpleMaterial("AntiqueWoodMat", furnitureColor, 0.4f);

            // 1. Floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Hardwood_Floor";
            floor.transform.SetParent(generatedRoomContainer.transform);
            floor.transform.localScale = new Vector3(roomWidth, 0.2f, roomLength);
            floor.transform.position = new Vector3(0, -0.1f, 0);
            floor.GetComponent<Renderer>().material = floorMat;

            // 2. Ceiling
            GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ceiling.name = "Ceiling";
            ceiling.transform.SetParent(generatedRoomContainer.transform);
            ceiling.transform.localScale = new Vector3(roomWidth, 0.2f, roomLength);
            ceiling.transform.position = new Vector3(0, roomHeight, 0);
            ceiling.GetComponent<Renderer>().material = wallMat;

            // 3. Walls
            CreateWall("BackWall", new Vector3(0, roomHeight / 2f, roomLength / 2f), new Vector3(roomWidth, roomHeight, 0.2f), wallMat);
            CreateWall("LeftWall", new Vector3(-roomWidth / 2f, roomHeight / 2f, 0), new Vector3(0.2f, roomHeight, roomLength), wallMat);
            CreateWall("RightWall", new Vector3(roomWidth / 2f, roomHeight / 2f, 0), new Vector3(0.2f, roomHeight, roomLength), wallMat);

            // 4. Interactive Study Desk & Laptop
            CreateStudyDesk(woodMat);

            // 5. Interactive Tea Table with Tea Set
            CreateTeaTable(woodMat);

            // 6. Interactive Bookshelf
            CreateBookshelf(woodMat);

            // 7. Ambient Mana Particles
            CreateManaParticles();

            // 8. Warm Lighting
            SetupLighting();
        }

        private void CreateWall(string name, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(generatedRoomContainer.transform);
            wall.transform.position = pos;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().material = mat;
        }

        private void CreateStudyDesk(Material woodMat)
        {
            GameObject desk = new GameObject("Interactive_CodingDesk");
            desk.transform.SetParent(generatedRoomContainer.transform);
            desk.transform.position = new Vector3(0, 0, 2.2f);

            // Desk top
            GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
            top.transform.SetParent(desk.transform, false);
            top.transform.localScale = new Vector3(1.6f, 0.08f, 0.8f);
            top.transform.localPosition = new Vector3(0, 0.74f, 0);
            top.GetComponent<Renderer>().material = woodMat;

            // Legs
            for (int x = -1; x <= 1; x += 2)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    leg.transform.SetParent(desk.transform, false);
                    leg.transform.localScale = new Vector3(0.08f, 0.74f, 0.08f);
                    leg.transform.localPosition = new Vector3(x * 0.72f, 0.37f, z * 0.32f);
                    leg.GetComponent<Renderer>().material = woodMat;
                }
            }

            var prop = desk.AddComponent<RemInteractiveProp>();
            prop.Interact(); // configure hook
        }

        private void CreateTeaTable(Material woodMat)
        {
            GameObject table = new GameObject("Interactive_TeaTable");
            table.transform.SetParent(generatedRoomContainer.transform);
            table.transform.position = new Vector3(1.8f, 0, 1.2f);

            GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            top.transform.SetParent(table.transform, false);
            top.transform.localScale = new Vector3(0.8f, 0.04f, 0.8f);
            top.transform.localPosition = new Vector3(0, 0.55f, 0);
            top.GetComponent<Renderer>().material = woodMat;

            // Teapot & cup
            GameObject teapot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            teapot.name = "Teapot";
            teapot.transform.SetParent(table.transform, false);
            teapot.transform.localScale = new Vector3(0.18f, 0.16f, 0.18f);
            teapot.transform.localPosition = new Vector3(0, 0.65f, 0);
            teapot.GetComponent<Renderer>().material = CreateSimpleMaterial("PorcelainMat", Color.white, 0.1f);

            var prop = table.AddComponent<RemInteractiveProp>();
        }

        private void CreateBookshelf(Material woodMat)
        {
            GameObject shelf = new GameObject("Interactive_Bookshelf");
            shelf.transform.SetParent(generatedRoomContainer.transform);
            shelf.transform.position = new Vector3(-2.4f, 1.2f, 1.8f);

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.transform.SetParent(shelf.transform, false);
            body.transform.localScale = new Vector3(1.2f, 2.2f, 0.4f);
            body.GetComponent<Renderer>().material = woodMat;

            var prop = shelf.AddComponent<RemInteractiveProp>();
        }

        private void CreateManaParticles()
        {
            GameObject pObj = new GameObject("Mana_Particles");
            pObj.transform.SetParent(generatedRoomContainer.transform);
            pObj.transform.position = new Vector3(0, 1.5f, 1.5f);

            manaEmbers = pObj.AddComponent<ParticleSystem>();
            var main = manaEmbers.main;
            main.startColor = new Color(0.4f, 0.75f, 1f, 0.6f); // Soft Rem mana cyan
            main.startSize = 0.04f;
            main.startSpeed = 0.15f;
            main.maxParticles = 50;

            var shape = manaEmbers.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(4f, 2.5f, 4f);
        }

        private void SetupLighting()
        {
            if (sunLight == null)
            {
                GameObject sunObj = new GameObject("SunLight_Window");
                sunObj.transform.SetParent(generatedRoomContainer.transform);
                sunLight = sunObj.AddComponent<Light>();
                sunLight.type = LightType.Directional;
                sunLight.color = new Color(1.0f, 0.96f, 0.88f);
                sunLight.intensity = 1.2f;
                sunObj.transform.rotation = Quaternion.Euler(45, -30, 0);
            }

            if (deskLampLight == null)
            {
                GameObject lampObj = new GameObject("Warm_DeskLamp");
                lampObj.transform.SetParent(generatedRoomContainer.transform);
                lampObj.transform.position = new Vector3(0, 1.4f, 2.0f);
                deskLampLight = lampObj.AddComponent<Light>();
                deskLampLight.type = LightType.Point;
                deskLampLight.color = new Color(1.0f, 0.82f, 0.65f);
                deskLampLight.range = 5.0f;
                deskLampLight.intensity = 1.8f;
            }
        }

        private Material CreateSimpleMaterial(string name, Color color, float roughness)
        {
            Material mat = new Material(Shader.Find("Standard") ?? Shader.Find("Universal Render Pipeline/Lit"));
            mat.name = name;
            mat.color = color;
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 1.0f - roughness);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 1.0f - roughness);
            return mat;
        }
    }
}
