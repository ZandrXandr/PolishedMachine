using Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PolishedMachine.ModUtilities.Extenders {
    public class CharacterExtender {


        public static Type playerType;
        public static CharacterExtender instance;

        public List<Type> playerTypes;
        public List<string> arts;
        public Dictionary<string, string> playerNames;
        public int currentPlayer;

        public Player playerObject;

        public bool isMenuInitialized = false;

        public MenuContainer newCharacterMenuContainer;
        public SlugcatSelectMenu currentMenu;

        public RoundedRect backgroundRect;
        public SimpleButton downButton;
        public SimpleButton upButton;

        public SimpleButton[] characterButtons;

        public FSprite customArtSprite;

        public int currentScrollIndex = 0;
        public int currentSelectedIndex = -1;

        public bool selectedCustom = false;

        public float artOffsetGoal = 0;
        public float artOffset = 0;

        public void Init() {

            if( instance != null )
                return;

            playerTypes = new List<Type>();
            playerNames = new Dictionary<string, string>();
            playerType = typeof( Player );
            arts = new List<string>();
            instance = this;


            On.ProcessManager.SwitchMainProcess += HookProcessSwitch;
            On.Menu.SlugcatSelectMenu.Singal += HookSingal;
            On.Menu.SlugcatSelectMenu.SlugcatPage.GrafUpdate += HookPageGrafUpdate;
            On.Menu.SlugcatSelectMenu.Update += (On.Menu.SlugcatSelectMenu.orig_Update o, Menu.SlugcatSelectMenu i) => { o( i ); UpdateMenu(); };

            On.AbstractCreature.Realize += HookRealize;
        }

        public void Update() {
            artOffset = Mathf.Lerp( artOffset, artOffsetGoal, 0.05f );
        }

        public int RegisterNewSlugcat(string playerName, Type newPlayerType) {

            //Check if the type is a player
            if( newPlayerType.BaseType.Name != playerType.Name ) {
                Debug.LogError( "Tried to register type " + newPlayerType.Name + " which isn't a player!" );
                return -1;
            }

            //Return the index of the player we're registering
            try {
                playerTypes.Add( newPlayerType );
                playerNames.Add( newPlayerType.Name, playerName );
                arts.Add( string.Empty );

                return playerTypes.Count - 1;
            } catch( System.Exception e ) {
                Debug.LogError( e );
                return -1;
            }
        }
        public void SetSlugcatArt(int playerIndex, string elementName) {
            arts[playerIndex] = elementName;

            try {
                Debug.Log( "Set slugcat " + playerTypes[playerIndex].Name + "'s art to " + elementName );
            } catch( Exception e ) {
                Debug.LogError( e );
            }
        }

        public void HookRealize(On.AbstractCreature.orig_Realize original, AbstractCreature instance) {
            try {
                if( selectedCustom && instance.creatureTemplate.TopAncestor().type == CreatureTemplate.Type.Slugcat ) {
                    Player newPlayer = Activator.CreateInstance( playerTypes[currentSelectedIndex], instance, instance.world ) as Player;
                    instance.realizedCreature = newPlayer;

                    playerObject = newPlayer;
                    Debug.Log( "Created Custom Slugcat " + newPlayer.GetType().Name );
                } else {
                    original( instance );
                    playerObject = (Player)instance.realizedObject;
                }
            } catch( System.Exception e ) {
                Debug.LogError( e );
                original( instance );
            }
        }
        public void HookProcessSwitch(On.ProcessManager.orig_SwitchMainProcess orig, ProcessManager instance, ProcessManager.ProcessID id) {

            if( instance.currentMainLoop != null ) {
                if( instance.currentMainLoop.ID == ProcessManager.ProcessID.Game ) {
                    Debug.Log( "Switching off of game" );

                    if( playerObject != null )
                        playerObject.Destroy();
                }
            }

            orig( instance, id );

            if( id == ProcessManager.ProcessID.SlugcatSelect ) {
                currentMenu = ( instance.currentMainLoop as SlugcatSelectMenu );
                newCharacterMenuContainer = new MenuContainer( currentMenu, currentMenu.pages[0], Vector2.zero );
                CreateMenu();
                currentMenu.pages[0].subObjects.Add( newCharacterMenuContainer );
                newCharacterMenuContainer.menu = currentMenu;
                UpdateMenu();
            } else if( isMenuInitialized ) {
                DestroyMenu();
            }
        }
        public void HookPageGrafUpdate(On.Menu.SlugcatSelectMenu.SlugcatPage.orig_GrafUpdate orig, SlugcatSelectMenu.SlugcatPage instance, float timeStacker) {
            float num = instance.Scroll( timeStacker );
            float num2 = instance.UseAlpha( timeStacker );
            float depth = instance.slugcatDepth;

            if( instance.HasMark ) {
                float num3 = Mathf.Lerp( instance.lastMarkAlpha, instance.markAlpha, timeStacker ) * num2;
                if( instance.slugcatNumber == 2 ) {
                    num3 *= ( ( !( instance is SlugcatSelectMenu.SlugcatPageContinue ) ) ? 0f : Mathf.Pow( Mathf.InverseLerp( 4f, 14f, (float)( instance as SlugcatSelectMenu.SlugcatPageContinue ).saveGameData.cycle ), 3.5f ) );
                }
                Vector2 a = new Vector2( instance.MidXpos + num * instance.ScrollMagnitude, instance.imagePos.y + 150f ) + instance.markOffset;
                a -= instance.slugcatImage.CamPos( timeStacker ) * 80f / depth;
                a -= new Vector2( 0, artOffset );
                instance.markSquare.x = a.x;
                instance.markSquare.y = a.y;
                instance.markSquare.alpha = Mathf.Pow( num3, 0.75f );
                instance.markGlow.x = a.x;
                instance.markGlow.y = a.y;
                instance.markGlow.scale = Mathf.Lerp( 3f, 3.3f, Mathf.Pow( num3, 0.2f ) ) + ( ( !instance.HasGlow ) ? 0f : -0.5f ) + Mathf.Lerp( -0.1f, 0.1f, UnityEngine.Random.value ) * instance.markFlicker;
                instance.markGlow.alpha = ( ( instance.slugcatNumber != 0 ) ? 0.6f : 0.4f ) * Mathf.Pow( num3, 0.75f );
            }
            if( instance.HasGlow ) {
                float num4 = Mathf.Lerp( 0.8f, 1f, Mathf.Lerp( instance.lastGlowAlpha, instance.glowAlpha, timeStacker ) ) * num2 * Mathf.Lerp( instance.lastInposition, instance.inPosition, timeStacker );
                Vector2 a2 = new Vector2( instance.MidXpos + num * instance.ScrollMagnitude, instance.imagePos.y ) + instance.glowOffset;
                a2 -= instance.slugcatImage.CamPos( timeStacker ) * 80f / depth;
                a2 -= new Vector2( 0, artOffset );
                instance.glowSpriteB.color = Color.Lerp( instance.effectColor, new Color( 1f, 1f, 1f ), 0.3f * num4 );
                instance.glowSpriteB.x = a2.x;
                instance.glowSpriteB.y = a2.y;
                instance.glowSpriteB.scale = Mathf.Lerp( 20f, 38f, Mathf.Pow( num4, 0.75f ) );
                instance.glowSpriteB.alpha = Mathf.Pow( num4, 0.25f ) * Mathf.Lerp( 0.394f, 0.406f, UnityEngine.Random.value * ( 1f - Mathf.Lerp( instance.lastGlowAlpha, instance.glowAlpha, timeStacker ) ) );
                instance.glowSpriteA.color = Color.Lerp( instance.effectColor, new Color( 1f, 1f, 1f ), 0.9f * num4 );
                instance.glowSpriteA.x = a2.x;
                instance.glowSpriteA.y = a2.y;
                instance.glowSpriteA.scale = Mathf.Lerp( 10f, 17f, Mathf.Pow( num4, 1.2f ) );
                instance.glowSpriteA.alpha = num4 * 0.6f;
            }
            for( int i = 0; i < instance.slugcatImage.depthIllustrations.Count; i++ ) {
                Vector2 a3 = instance.slugcatImage.depthIllustrations[i].pos;
                a3 -= new Vector2( 0, artOffset );
                a3 -= instance.slugcatImage.CamPos( timeStacker ) * 80f / instance.slugcatImage.depthIllustrations[i].depth;
                a3 += instance.sceneOffset;
                a3.x += num * instance.ScrollMagnitude;
                instance.slugcatImage.depthIllustrations[i].sprite.x = a3.x;
                instance.slugcatImage.depthIllustrations[i].sprite.y = a3.y;
                instance.slugcatImage.depthIllustrations[i].sprite.alpha = instance.slugcatImage.depthIllustrations[i].alpha * num2;
            }
            for( int j = 0; j < instance.slugcatImage.flatIllustrations.Count; j++ ) {
                Vector2 a4 = instance.slugcatImage.flatIllustrations[j].pos;
                a4 -= new Vector2( 0, artOffset );
                a4 += instance.sceneOffset;
                a4.x += num * instance.ScrollMagnitude;
                instance.slugcatImage.flatIllustrations[j].sprite.x = a4.x;
                instance.slugcatImage.flatIllustrations[j].sprite.y = a4.y;
                instance.slugcatImage.flatIllustrations[j].sprite.alpha = instance.slugcatImage.flatIllustrations[j].alpha * num2;
            }

            if( instance.slugcatImage.depthIllustrations.Count > 0 ) {
                int lastIndex = instance.slugcatImage.depthIllustrations.Count - 1;

                if( customArtSprite != null ) {
                    customArtSprite.RemoveFromContainer();
                    Vector2 a3 = new Vector2( Futile.screen.width / 2, Futile.screen.height / 2 );
                    a3 += new Vector2( 0, ( Futile.screen.height * 2 ) - artOffset );
                    customArtSprite.x = a3.x;
                    customArtSprite.y = a3.y;
                    customArtSprite.width = 400f;
                    customArtSprite.height = 400f;
                    customArtSprite.y += customArtSprite.height / 6;
                    instance.slugcatImage.depthIllustrations[lastIndex].sprite.container.AddChild( customArtSprite );
                    customArtSprite.MoveToBack();
                    customArtSprite.MoveInFrontOfOtherNode( instance.slugcatImage.depthIllustrations[lastIndex].sprite );
                }
            }
        }
        public void HookSingal(On.Menu.SlugcatSelectMenu.orig_Singal original, SlugcatSelectMenu instance, MenuObject sender, string message) {
            if( message != null ) {
                if( message == "CHARACTERDOWN" ) {
                    currentScrollIndex--;
                } else if( message == "CHARACTERUP" ) {
                    currentScrollIndex++;
                } else if( message[0] == 'C' ) {
                    SelectedPlayer( int.Parse( message[1].ToString() ) );
                } else if( message == "START" ) {
                    original( instance, sender, message );
                    customArtSprite.isVisible = false;
                } else {
                    original( instance, sender, message );
                }
            } else {
                original( instance, sender, message );
            }
        }

        public void CreateMenu() {
            int buttonCount = 2 + 6;
            float buttonHeight = 30f;
            float boxHeight = ( ( 5 + 5 + 30f ) * 2 ) + ( ( buttonHeight + 5 ) * ( buttonCount - 2 ) );
            characterButtons = new SimpleButton[buttonCount - 2];

            backgroundRect = new RoundedRect( currentMenu, currentMenu.pages[0], new Vector2( Futile.screen.width - 220f, Futile.screen.height - boxHeight - 20f ), new Vector2( 200f, boxHeight ), false );
            currentMenu.pages[0].subObjects.Add( backgroundRect );

            currentMenu.pages[0].subObjects.Add( downButton = new SimpleButton( currentMenu, backgroundRect, "Down", "CHARACTERDOWN", new Vector2( 5, 5 ), new Vector2( 190f, 30f ) ) );

            for( int i = 0; i < buttonCount - 2; i++ ) {
                currentMenu.pages[0].subObjects.Add( characterButtons[i] = new SimpleButton( currentMenu, backgroundRect, "", "C" + i, downButton.pos + new Vector2( 0, downButton.size.y + 7 ) + new Vector2( 0, ( 30f + 5 ) * i ), new Vector2( 190f, 30f ) ) );
            }

            currentMenu.pages[0].subObjects.Add( upButton = new SimpleButton( currentMenu, backgroundRect, "Up", "CHARACTERUP", new Vector2( 5, boxHeight - 5f - 30f ), new Vector2( 190f, 30f ) ) );
            customArtSprite = new FSprite( "Futile_White" );
            isMenuInitialized = true;
        }
        public void DestroyMenu() {
            customArtSprite.RemoveFromContainer();
            characterButtons = null;
            backgroundRect = null;
            downButton = null;
            upButton = null;
            isMenuInitialized = false;
        }
        public void UpdateMenu() {

            if( characterButtons == null )
                return;

            for( int i = 0; i < characterButtons.Length; i++ ) {
                int realIndex = i + currentScrollIndex;
                SimpleButton button = characterButtons[i];

                if( realIndex >= playerTypes.Count ) {
                    button.menuLabel.text = string.Empty;
                } else if( realIndex >= 0 ) {
                    try {

                        button.menuLabel.text = playerNames[playerTypes[realIndex].Name];

                        if( currentSelectedIndex == realIndex && selectedCustom ) {
                            button.menuLabel.text = button.menuLabel.text.Insert( 0, "->" );
                            button.menuLabel.text += "<-";
                        }
                    } catch( System.Exception e ) {
                        Debug.LogError( "BAD" + e );
                    }
                } else {
                    button.menuLabel.text = string.Empty;
                }
            }
        }

        public void SelectedPlayer(int index) {
            int realIndex = index + currentScrollIndex;

            if( realIndex >= playerTypes.Count ) {
                selectedCustom = false;
                artOffsetGoal = 0;
                return;
            }

            if( realIndex == currentSelectedIndex ) {
                currentSelectedIndex = -1;
                selectedCustom = false;
                artOffsetGoal = 0;
            } else {
                selectedCustom = true;
                currentSelectedIndex = realIndex;

                if( !string.IsNullOrEmpty(arts[currentSelectedIndex]) ) {
                    artOffsetGoal = Futile.screen.height * 2;
                    SetNewArt();
                }
            }

        }

        public void SetNewArt() {
            customArtSprite.MoveToBack();
            customArtSprite.SetElementByName( arts[currentSelectedIndex] );
            Debug.Log( customArtSprite.element + "|" + customArtSprite.width );
        }

    }
}
