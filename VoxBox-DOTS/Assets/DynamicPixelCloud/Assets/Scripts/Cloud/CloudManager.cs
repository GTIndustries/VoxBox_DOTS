namespace DynamicPixelCloud {
    using UnityEngine;
    using System.Collections;
    using Unity.Entities;
    using Unity.Collections;
    using System;
    using Unity.Transforms;
    using Unity.Mathematics;
    using Random = UnityEngine.Random;

    /// <summary>
    /// The CloudManager class.
    /// </summary>
    public class CloudManager : MonoBehaviour {
        /// <summary>
        /// The CloudManager single instance.
        /// </summary>
        public static CloudManager inst;

        [Tooltip("The cloud unit prefab.")] public GameObject cloudPrefab;

        [Tooltip(
            "Checks it to let the cloud system run in ECS mode. Unchecks it to let the cloud system run in normal mode."
        )]
        public bool ECS = true;

        [Tooltip("Determines the cloud area. Increase the rows and columns will get a larger area of clouds.")]
        public int rows = 80;

        [Tooltip("Determines the cloud area. Increase the rows and columns will get a larger area of clouds.")]
        public int columns = 80;

        [Range(0.3f, 0.7f)] [Tooltip("Controls the density of the clouds.")]
        public float density = 0.45f;

        [Range(2, 6)] [Tooltip("The maximum size a cloud unit could expand to.")]
        public float maximumSize = 2f;

        [Tooltip("The transform scale multiplier will multiply on the localscale of each cloud unit.")]
        public Vector3 transformScaleMultiplier = new Vector3(1.5f, 1f, 1.5f);

        [Range(5, 10)] [Tooltip("Controls the chaos of the clouds.")]
        public float chaos = 6f;

        [Tooltip("Controls the cloud move speed.")]
        public float moveSpeed = 0.03f;

        [Tooltip("Controls the cloud unit expand speed.")]
        public float expandSpeed = 0.4f;

        [Tooltip("Controls the cloud unit contract speed.")]
        public float contractSpeed = 0.4f;

        [Tooltip("Controls the cloud move direction.")]
        public Vector2 windDirection = new Vector2(1f, 1f);

        /// <summary>
        /// The cloud units list.
        /// </summary>
        private GameObject[] clouds;

        /// <summary>
        /// The cloud data list.
        /// </summary>
        private CloudPerlinNoiseData[] data;

        /// <summary>
        /// The entity manager.
        /// </summary>
        private EntityManager entityManager;

        /// <summary>
        /// The cloud unit prefab entity.
        /// </summary>
        private Entity cloudPrefabEntity;

        /// <summary>
        /// The total run time.
        /// </summary>
        private float totalTime = 0;

        /// <summary>
        /// Records if it's in ECS mode at the begining of play mode.
        /// </summary>
        private bool isECSWhenStart = true;

        /// <summary>
        /// Enters the Awake stage.
        /// </summary>
        private void Awake() {
            if (CloudManager.inst != null && CloudManager.inst != this) {
                Destroy(this);
                return;
            }

            CloudManager.inst   = this;
            this.isECSWhenStart = this.ECS;
        }

        /// <summary>
        /// Enters the Start stage.
        /// </summary>
        private void Start() {
            if (this.isECSWhenStart) {
                this.InitECS();
            }
            else {
                this.InitNoUseECS();
            }
        }

        /// <summary>
        /// Initializes the clouds run in non-ECS mode.
        /// </summary>
        private void InitNoUseECS() {
            this.clouds = new GameObject[this.rows * this.columns];
            this.data   = new CloudPerlinNoiseData[this.clouds.Length];
            var parentPos       = this.transform.position;
            var parentTransform = this.transform;
            int index           = 0;

            for (int i = 0; i < this.rows; ++i) {
                for (int j = 0; j < this.columns; ++j) {
                    GameObject gameObject = Instantiate(this.cloudPrefab, parentTransform);
                    this.clouds[index] = gameObject;
                    Vector3 position = new Vector3((float)i, 0f, (float)j) + parentPos;
                    this.data[index].offsetX        = position.x;
                    this.data[index].offsetZ        = position.z;
                    this.data[index].chaos          = Random.value;
                    this.data[index].scale          = 0f;
                    gameObject.transform.position   = position;
                    gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    ++index;
                }
            }

            StartCoroutine(this.UpdateCloudsData());
        }

        /// <summary>
        /// Initializes the clouds run in ECS mode.
        /// </summary>
        private void InitECS() {
            this.entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            Debug.Log("Initializing clouds");
            using (var blobAssetStore = new BlobAssetStore()) {
                this.cloudPrefabEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                    this.cloudPrefab,
                    GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore)
                );

                NativeArray<Entity> clouds = new NativeArray<Entity>(this.rows * this.columns, Allocator.Temp);
                this.entityManager.AddComponent<NonUniformScale>(this.cloudPrefabEntity);
                this.entityManager.Instantiate(this.cloudPrefabEntity, clouds);

                var parentPos = this.transform.position;
                int index     = 0;

                for (int i = 0; i < this.rows; ++i) {
                    for (int j = 0; j < this.columns; ++j) {
                        float3 pos = new float3((float)i, 0f, (float)j) + (float3)parentPos;
                        this.entityManager.SetComponentData(
                            clouds[index],
                            new Translation {
                                Value = pos
                            }
                        );
                        this.entityManager.SetComponentData(
                            clouds[index],
                            new NonUniformScale {
                                Value = new float3(0.5f, 0.5f, 0.5f)
                            }
                        );
                        this.entityManager.SetComponentData(
                            clouds[index],
                            new CloudPerlinNoiseData {
                                row     = i,
                                col     = j,
                                scale   = 0,
                                offsetX = pos.x,
                                offsetZ = pos.z,
                                chaos   = Random.value
                            }
                        );
                        ++index;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the Clouds data.
        /// </summary>
        /// <returns>The IEnumerator.</returns>
        private IEnumerator UpdateCloudsData() {
            while (true) {
                int index       = 0;
                var dir         = windDirection.normalized;
                var realDensity = 1 - this.density;

                for (int i = 0; i < this.rows; ++i) {
                    for (int j = 0; j < this.columns; ++j) {
                        float x         = i / (float)this.rows;
                        float y         = j / (float)this.columns;
                        float frequency = this.chaos + this.data[index].chaos * 0.3f;
                        float x1        = this.totalTime * dir.x + x * frequency;
                        float y1        = this.totalTime * dir.y + y * frequency;
                        float x2        = this.totalTime * dir.x + x * frequency * 0.5f;
                        float y2        = this.totalTime * dir.y + y * frequency * 0.5f;
                        float scale     = Mathf.PerlinNoise(x1, y1);
                        float visible   = Mathf.PerlinNoise(x2, y2);

                        float targetScale = 0f;

                        if (scale > realDensity && visible > 0.5f) {
                            targetScale = this.maximumSize * scale;
                        }

                        if (this.data[index].scale < targetScale) {
                            this.data[index].scale += this.totalTime == 0 ? targetScale : this.expandSpeed * 0.02f;
                            this.data[index].scale =  Mathf.Min(targetScale, this.data[index].scale);
                        }
                        else {
                            this.data[index].scale -= this.totalTime == 0 ? targetScale : this.contractSpeed * 0.02f;
                            this.data[index].scale =  Mathf.Max(targetScale, this.data[index].scale);
                        }

                        this.data[index].offsetX = Mathf.Min(Mathf.Max(this.data[index].offsetX, -0.5f), 0.5f);
                        this.data[index].offsetZ = Mathf.Min(Math.Max(this.data[index].offsetZ, -0.5f), 0.5f);
                        ++index;
                    }
                }

                if (this.totalTime >= 100f) {
                    this.totalTime = 0;
                }

                this.totalTime += Time.deltaTime * moveSpeed * 0.5f;
                yield return new WaitForSeconds(0.001f);
            }
        }

        /// <summary>
        /// Updates the clouds run in non-ECS mode.
        /// </summary>
        private void UpdateNoUseECS() {
            var parentPos = base.transform.position;
            int index     = 0;

            for (int i = 0; i < this.rows; ++i) {
                for (int j = 0; j < this.columns; ++j) {
                    var scale = this.data[index].scale;
                    this.clouds[index].transform.localScale = scale * this.transformScaleMultiplier;
                    this.clouds[index].transform.position =
                        new Vector3((float)i, 0f, (float)j)
                      + parentPos
                      + new Vector3(this.data[index].offsetX, 0, this.data[index].offsetZ) * 0.5f;
                    ++index;
                }
            }
        }

        /// <summary>
        /// Enters the Update stage.
        /// </summary>
        private void Update() {
            if (!this.isECSWhenStart) {
                this.UpdateNoUseECS();
            }
        }
    }
}