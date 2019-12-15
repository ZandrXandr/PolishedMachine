using PolishedMachine.ModUtilities.Extenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PolishedMachine.ModUtilities {
    public class UtilityScript : MonoBehaviour{

        public List<Room> loadedRooms = new List<Room>();
        public List<PhysicalObject> physObjects = new List<PhysicalObject>();
        public List<Player> players = new List<Player>();

        private float updateTimer = 0.1f;

        public RainWorld rainworldInstance;

        public static Dictionary<KeyCode, List<Action>> KeyCheckers = new Dictionary<KeyCode, List<Action>>();
        public static HashSet<KeyCode> pressedKeys = new HashSet<KeyCode>();

        public void Update() {
            CheckInputs();

            if( this.rainworldInstance == null ) {
                this.rainworldInstance = FindObjectOfType<RainWorld>();
                return;
            }
            this.updateTimer -= Time.deltaTime;
            if( this.updateTimer <= 0f ) {
                this.UpdateLists();
                this.updateTimer = 0.1f;
            }

            CharacterExtender.instance.Update();
        }

        public void UpdateLists() {
            this.UpdateRoomList();
            this.UpdatePhysicalObjects();
            this.UpdatePlayers();
        }
        public void UpdateRoomList() {
            if( this.rainworldInstance.processManager.currentMainLoop is RainWorldGame ) {
                RainWorldGame rainWorldGame = this.rainworldInstance.processManager.currentMainLoop as RainWorldGame;
                this.loadedRooms.Clear();
                this.loadedRooms.AddRange( rainWorldGame.world.activeRooms );
            }
        }
        public void UpdatePhysicalObjects() {
            this.physObjects.Clear();
            foreach( Room room in this.loadedRooms ) {
                foreach( List<PhysicalObject> collection in room.physicalObjects ) {
                    this.physObjects.AddRange( collection );
                }
            }
        }
        public void UpdatePlayers() {
            this.players.Clear();
            foreach( PhysicalObject physicalObject in this.physObjects ) {
                if( physicalObject is Player ) {
                    this.players.Add( physicalObject as Player );
                }
            }
        }

        public static void RegisterInputChecker(KeyCode code, Action action) {
            if( !KeyCheckers.ContainsKey( code ) )
                KeyCheckers[code] = new List<Action>();
            KeyCheckers[code].Add( action );
        }
        public static void RemoveInputChecker(KeyCode code, Action action) {
            if( !KeyCheckers.ContainsKey( code ) )
                return;
            KeyCheckers[code].Remove( action );
        }
#pragma warning disable CA1822 // Mark members as static
        public void CheckInputs() {
            foreach ( KeyValuePair<KeyCode, List<Action>> kvp in KeyCheckers ) {
                if( Input.GetKey( kvp.Key ) ) {
                    if( !pressedKeys.Contains( kvp.Key ) ) {
                        pressedKeys.Add( kvp.Key );

                        foreach( Action a in kvp.Value ) {
                            try {
                                a.Invoke();
                            } catch( System.Exception e ) {
                                Debug.LogError( e );
                            }
                        }
                    }
                } else if( pressedKeys.Contains( kvp.Key ) ) {
                    pressedKeys.Remove( kvp.Key );
                }
            }
        }
    }
}
