using OWML.ModHelper;
using OWML.Common;
using UnityEngine;

namespace SpaceAnglers
{
    public class SpaceAnglers : ModBehaviour
    {
        public static SpaceAnglers Instance { get; set; }

        private ShipBody ship;
        private PlayerBody player;
        private GameObject angler;
        public GameObject copyAngler;
        private Transform worldRoot;

        private void Awake()
        {

        }

        private void Start()
        {
            Instance = this;

            ModHelper.HarmonyHelper.EmptyMethod<AnglerfishController>("ApplyDrag", true);
            //ModHelper.HarmonyHelper.EmptyMethod<AnglerfishController>("OnSectorOccupantsUpdated", true); 
            //ModHelper.HarmonyHelper.EmptyMethod<AnglerfishController>("OnSectorOccupantRemoved", true);

            ModHelper.HarmonyHelper.AddPostfix<AnglerfishController>("OnSectorOccupantsUpdated", typeof(SpaceAnglers), "SectorSpam");

            ModHelper.Events.Subscribe<AnglerfishController>(Events.AfterStart);
            ModHelper.Events.Event += OnAnglerEvent;

            // Example of accessing game code.
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.SolarSystem) return;
                ship = FindObjectOfType<ShipBody>();
                player = FindObjectOfType<PlayerBody>();
                worldRoot = GameObject.Find("SolarSystemRoot").transform;

                if (angler != null)
                {
                    SpawnAnglerFish();
                }
            };
        }

        private void OnAnglerEvent(MonoBehaviour controller, Events ev)
        {
            if (ev != Events.AfterStart) return;

            angler = controller.gameObject;

            SpawnAnglerFish();

            ModHelper.Events.Event -= OnAnglerEvent;
        }

        public void SpawnAnglerFish()
        {
            copyAngler = Instantiate(angler, ship.transform);

            AnglerfishController controller = copyAngler.GetComponent<AnglerfishController>();
            controller._sector = Instance.ship.GetComponentInChildren<Sector>();
            controller._brambleBody = Instance.ship;

            copyAngler.transform.localPosition = Vector3.down;
        }

        private static void SectorSpam()
        {
            Instance.ModHelper.Console.WriteLine(Instance.copyAngler.activeSelf.ToString(), MessageType.Info);
        }
    }
}
