using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LyricsController : MonoBehaviour
{
    public GameObject changingLyrics;
    public Text lyricsText;
    private Color lyricsColor;
    private string currentLyricsIndex;
    string sceneName;
    List<string> wisdomyLyricsList = new List<string>();
    List<string> militaryLyricsList = new List<string>();
    List<string> mournfulLyricsList = new List<string>();
    List<string> partyLyricsList = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        lyricsColor = lyricsText.color;  //  sets color to object
        lyricsColor.a = 0.0f;// changes the color of alpha
        lyricsText.color = lyricsColor;

        // Create a temporary reference to the current scene.
        Scene currentScene = SceneManager.GetActiveScene();

        // Retrieve the name of this scene.
        sceneName = currentScene.name;

        Debug.Log(SceneManager.GetActiveScene().name);

		// Finding which lyrics to use
		//Wisdom
        wisdomyLyricsList.Add("The wise can often profit by the lessons of a foe, for caution is the mother of safety. " +
            "It is just such a thing as one will not learn from a friend and which an enemy compels you to know. " +
            "To begin with, it's the foe and not the friend that taught cities to build high walls, to equip long vessels of war; " +
            "and it's this knowledge that protects our children, our slaves and our wealth. " +
            "(Aristophanes’ Birds, Epops)");
        wisdomyLyricsList.Add("Health is best for a mortal man, " + 
			"Second, to be beautiful in stature, " +
            "Third to be wealthy without guile, " +
            "And fourth to be young with one’s friends " + 
			"(PMG 890, tr.G.Jones)");
        wisdomyLyricsList.Add("The crab spoke thus " +
            "Taking the snake in its claw, " +
            "‘a comrade ought to be straight " +
            "And not think crooked thoughts’ " + 
			"(PMG 892, tr.G.Jones)");
        wisdomyLyricsList.Add("If only it were possible to see what everyone is really like " +
            "By opening his breast, and looking at his mind " +
            "To close it back again  " +
            "And consider the man to be a friend with an honest heart " +
            "(PMG 889, tr.G.Jones)");
        wisdomyLyricsList.Add("Would that you, blind wealth, " +
            "Never on earth, nor sea " +
            "Nor land had appeared, " +
            "But dwell instead in Tartarus and " +
            "Acheron; because of you there " +
            "Is every kind of evil for mankind " +
            "(Timokreon of Rhodes, PMG 731, tr.G.Jones)");
        wisdomyLyricsList.Add("Some there are who say that the fairest thing seen " +
            "On the black earth is an array of horsemen; " +
            "Some, men, marching; some would say ships; but I say " +
            "She whom one loves best " +
            "Is the loveliest.Light were the work to make this " +
            "Plain to all, since she, who surpassed in beauty " +
            "All mortality, Helen, once forsaking " +
            "Her lordly husband, " +
            "Fled away to Troy-land across the water. " +
            "Not the thought of child nor beloved parents " +
            "Was remembered, after the Queen of Cyprus " +
            "Won her at first sight. " +
            "(Sappho 3, tr.Lattimore)");
        wisdomyLyricsList.Add("It is easier to beget and rear a man than to put good sense in him. " +
            "No one has yet devised a means whereby one has made the fool wise and a noble man out of one who is base. " +
            "(Theognis 429-431)");
        wisdomyLyricsList.Add("But 'tis with good intent to thee, friend Cyrnus, that " +
            "I shall give thee the counsels which I learnt from good men in my own childhood. " +
            "Be thou wise and draw to thyself neither honors nor virtues nor substance " +
            "on account of dishonourable or unrighteous deeds. " +
            "This then I would have thee to know, nor to consort with the bad but ever to cleave unto the good, " +
            "and at their tables to eat and to drink, and with them to sit,11 and them to please, " +
            "for their power is great.12 Of good men shalt thou learn good, but if thou mingle with the bad, " +
            "thou shalt e'en lose the wit thou hast already. Consort therefore with the good, and someday thou'lt say that I counsel my friends aright. " +
            "(Theognis 27 - 38) (Loeb)");
        wisdomyLyricsList.Add("Kyrnos my friend, Be flexible in character, always adapting, " +
            "Your own mood to that of the friend you chance to be with;, " +
            "Be as the lithe and tentacled octopus, altering color, " +
            "So that it matches and loses itself in the rock where it clings;, " +
            "So be you; be now like this, then change your complexion;, " +
            "Better you should be subtle than stubbornly always the same, " +
            "(Theognis 213 - 218)");
        wisdomyLyricsList.Add("There is one story " +
            "That Virtue has her dwelling place above rock walls hard to climb " +
            "With a grave chorus of light-footed nymphs attendant about her, " +
            "And she is not to be looked upon by the eyes of every mortal, " +
            "Only by one who with sweat, with clenched concentration " +
            "And courage, climbs to the peak. " +
            "(Simonides of Ceos 5, tr.Lattimore)");
        wisdomyLyricsList.Add("To be a good man, without blame and without question, " +
            "Foursquare founded hand and foot, mind also " +
            "Faultless fashioned, is difficult. " +
            "Thus the word of Pittakos " +
            "(Simonides of Ceos 6, tr.Lattimore)");
        wisdomyLyricsList.Add("While you live, shine " +
            "And have no grief whatsoever. " +
            "Life exists for only a moment " +
            "And Time demands his toll. " +
            "(Epitaph of Seikolos, 100BC - 200 AD, found near Ephesus)");
        //Military
        militaryLyricsList.Add("Poseidon, god of the racing steeds, I salute you, you who delight in their neighing and in the resounding clatter of their brass - shod hoofs, god of the swift galleys, which, loaded with mercenaries, cleave the seas with their azure beaks, god of the equestrian contests, in which young rivals, eager for glory, ruin themselves for the sake of distinction with their chariots in the arena, come and direct our chorus; Poseidon with the trident of gold, you, who reign over the dolphins, who are worshipped at Sunium and at Geraestus beloved of Phormio, and dear to the whole city above all the immortals, I salute you! (Aristophanes Knights, first semi - chorus)"
			);
        militaryLyricsList.Add("We will sing likewise the exploits of our steeds! they are worthy of our praises; in what invasions, what fights have I not seen them helping us! But especially admirable were they, when they bravely leapt upon the galleys, taking nothing with them but a coarse wine, some cloves of garlic and onions; despite this, they nevertheless seized the sweeps just like men, curved their backs over the thwarts and shouted,  " +
            "\"Hippapai!Give way!Come, all pull together!Come, come!How!Samphoras!Are you not rowing ?" + "\"" +
            "\" They rushed down upon the coast of Corinth, and the youngest hollowed out beds in the sand with their hoofs or went to fetch coverings; instead of luzern, they had no food but crabs, which they caught on the strand and even in the sea; so that Theorus causes a Corinthian crab to say,  " +
   "\"'Tis a cruel fate, oh Posidon neither my deep hiding-places, whether on land or at sea, can help me to escape the Knights." + "\"" + 
   " (Aristophanes Knights, second semi-chorus)");
        militaryLyricsList.Add("I am two things: a fighter who follows the Master of Battles, " +
        "And one who understands the gift of the Muses’ love(Archilochus 1) " +
        "It is my spear that makes my bread, my spear " +
        "That wins me my wine; and I lean on " +
        "My spear as I drink it! " +
        "(Archilochus 2)");
        militaryLyricsList.Add("Some barbarian fighter is waving my shield about, since I had to Leave that it behind, under a bush, though it was in perfectly good Shape. But what does it matter? I escaped with my life! Let the shield go – I’ll buy another one! (Archilochus 3)");
        militaryLyricsList.Add("Eternal Clouds!Let us arise to view with our dewy, clear - bright nature, from loud-sounding Father Ocean to the wood - crowned summits of the lofty mountains, in order that we may behold clearly the far - seen watch - towers, and the fruits, and the fostering, sacred earth, and the rushing sounds of the divine rivers, and the roaring, loud - sounding sea; for the unwearied eye of Aether sparkles with glittering rays.Come, let us shake off the watery cloud from our immortal forms and survey the earth with far - seeing eye. (Aristophanes Birds, 275 - 280, tr.Hall and Geldart)");
        militaryLyricsList.Add("In Delos Leto once bore children, Golden - haired Phoebus, lord Apollo, And the shooter of deer, the huntress Artemis, who holds great power over women(PMG 886, tr.G.Jones) ");
        //Mournful
        mournfulLyricsList.Add("Like a blacksmith the Love God has hammered me and crushed me On his anvil, and has crushed me in a winter torment (Anacreon of Teos 3, tr.Lattimore)");
        mournfulLyricsList.Add("Many in truth are your comrades when there's food and drink, but not so many when the enterprise is serious. (Theognis 115-116) Enjoy your youth, my dear heart: soon it will be the turn of other men, and I'll be dead and become dark earth. (Theognis 877-878) " + "\"" + "Two demons of drink beset wretched mortals, enfeebling thirst and harsh drunkenness. I'll steer a middle course between them and you won't persuade me either not to drink or to drink too much." + "\"" + " (Theognis 837 - 840)");
        mournfulLyricsList.Add("O wretched poverty, why do you delay to leave me and go to another man? Don't be attached to me against my will, but go, visit another house, and don't always share this miserable life with me. (Theognis 351-354) Don't show affection for me in your words but keep your mind and heart elsewhere, if you love me and the mind within you is loyal. Either love me sincerely or renounce me, hate me and quarrel openly (Theognis 87-90)");
        mournfulLyricsList.Add("To all to whom there is pleasure in song and to people yet unborn You also will be a song, while the earth and sun remain, Yet I am treated by you without even the least mark of respect And, as if I were a child, you have deceived me with words. (Theognis 251 - 254) (6th c BC)");
        mournfulLyricsList.Add("Best of all for mortal beings is never to have been born at all Nor ever to have set eyes on the bright light of the sun But, since he is born, a man should make utmost haste through the gates of Death And then repose, the earth piled into a mound round himself. (Theognis 425 - 428)");
        mournfulLyricsList.Add("I have been in my time as far as the land of Sicily. I have been to Euboia, where vineyards grow in the plain, And Sparta, the shining city by the reedy banks of Eurotas; And everywhere I was met with enthusiasm and love, But my heart has taken no joy from the attentions of strangers. A man’s own country is dearest.This is the truth in the end. (Theognis 783 - 88, tr.Lattimore)");
        mournfulLyricsList.Add("Being no more than a man, you cannot tell what will happen tomorrow, Nor, when you see one walk in prosperity know for how much time it will be. For overturn on the light - lifting wings of a dragonfly Is not more swift. (Simonides of Ceos 3, tr.Lattimore)");
        mournfulLyricsList.Add("No longer, maiden voice sweet-calling, sounds of allurement, Can my limbs bear me up; oh I wish, I wish I could be a seabird Who with halcyons skims the surf - flowers of the sea water With careless heart, a sea-blue - colored and sacred waterfowl (Alman of Sparta 3, tr.Lattimore)");
        //Party
        partyLyricsList.Add("Drink with me. Be young with me. Love with me. Wear crowns with me (PMG 901) Be crazy with me when I’m crazy! Be sensible when I have sense(PMG 902) Drink and get drunk with me, my friend Melanippos! Why would you say that once you cross the great eddying River of Acheron you will see the pure light of the sun again ? (Alcaeus Fr. 38A)");
        partyLyricsList.Add("Whom do we most praise? The man who sings The great thoughts that flow into his mind, and reflect his good heart. We’ve no use for stories of Titans and Giants Or centaurs, figments of our fathers’ imaginations! (Xenophanes of Colophon 1. 19 - 22)");
        partyLyricsList.Add("Here I go again! Golden Desire (she is blonde) Hits me squarely with a purple ball, Calling me out to play With that young thing wearing nice sandals…. (Anacreon Fr. 358)");
        partyLyricsList.Add("Ah, but I wish I were a large bit of gold And a beautiful someone would wear me with a pure mind!(PMG 900) O Pan, ruler of glorious Arcadia, Dancer, companion of the Bacchic Nymphs Laugh, o Pan, at my merriment Rejoicing in my song (PMG 887, tr.G.Jones)");
        partyLyricsList.Add("See, I have given you wings on which to hover uplifted High above earth entire and the great waste of the sea Without strain.Wherever men meet in festivals, as men Gather, you will be there, your name will be spoken again As the young singers, with the flutes clear piping beside them, Make you into a part of the winsome verses, and sing Of you. (Theognis  237 - 243, tr.Lattimore)");
        partyLyricsList.Add("Zeus rains upon us, and from the sky comes down Enormous winter.Rivers have turned to ice… Dash down the winter.Throw a log on the fire And mix the flattering wine(do not water it Too much), and bind on round our foreheads Soft ceremonial wreaths of spun fleece. We must not let our spirits give way to grief. By being sorry we get no further on, (Alcaeus of Mytilene 4, tr.Lattimore)");
        partyLyricsList.Add("Greeting oh Muse! Who dwells in trees and glades, And in my drinking cup – chirpity chirp chirp! I may sit on a stump still Green with leaves, and in hill and dale I’ll sing my praise - chirpity chirp chirp! And with a little brown beak, Like a little brown bird, I’ll sing for the delight of goaty foot Pan, And the Mountain Mother who makes us dance, Chirrripity chirp chirp! And I’ll sing for the places where Bees like to drink, the ambrosial Flowers, that may make them sing, Buzzety buzz buzzy, buzzety buzz! (adapted from Phrynichus, in Aristophanes Birds)");
        partyLyricsList.Add("Was there ever a time That the god of drinking Was not the object Of our grateful thanks ? The one wearing ivy, the one carrying a wand, Greeks have told his tales ever since Leaves learned to bud, and vines learned to grow, Reaching up to the sky and giving birth to Oodles of little grape babies, who never speak Till they are crushed, and milked, and turned into The drink all men may share – and then they have their own babies, Feasting, friendship and dance. Let my life be long, and my garlands green! (adapted from Ion of Chios, CURFRAG.tlg - 0308.1)");
        partyLyricsList.Add("All hail our potentate, our savior in a pinch, our ruler of the moment! Let the drinks be blended, Let the cups be silver, Let our hands be washed with wine And offerings be poured: First to Zeus, then to Herakles and Alkmena, Then to Prokles and the children of Perseus, And let’s drink, let’s play, let’s sing songs that rise Into the night air above our heads; Let’s dance a fling, Let’s make friends. (adapted from Ion of Chios, CURFRAG.tlg - 0308.2)");
        partyLyricsList.Add("Receive my toast, friend Theodorus – A cup full of words!In it are The graces themselves, mingled with scraps of Scribbled on paper.Now promise me Songs in return, so that Our feasts may be light and your happiness greater! (adapted from Dionysius the Brazen, CURFRAG.tlg - 0246.1)");
        partyLyricsList.Add("It’s time for love-struck young men To add a third kind of kottabos To the noisy ranks of the drinking god! Everybody twist your fingers into the handles of your cups, And count in your minds how much space lies Between your drinking couch and the goal – then let it fly! (adapted from Dionysius the Brazen, CURFRAG.tlg - 0246.3)");
        partyLyricsList.Add("You sweet old weaver of girly songs, Party maker, cheater of women, Rival of flute playing, Lover of lyres, the delightful and the dull Loving you will never grow old or die, Anacreon my friend, As long as servants serve drinks, And waiters bring food, Dances last all night, And the kottabos - pole stands ready for the flicking of wine drops! (adapted from Critias, CURFRAG.tlg - 0319.9)");
        partyLyricsList.Add("Now the floor is swept clean, every guest’s hands Are washed, the cups are shining.One settles garlands on his head, puts on his garlands, Another passes sweet smelling  myrrh on a dish.The mixing bowl Is close at hand and ready for service, full of the spirit of cheer, And still more drink is ready and will not disappoint, Sweet and strong, in earthen jars, holding its fragrance in reserve (Xenophanes of Colophon 1.1 - 7)");

        if (sceneName == "PartySongScene")
        {
            currentLyricsIndex = wisdomyLyricsList[Random.Range(0, wisdomyLyricsList.Count)];
        }
        if (sceneName == "SorrowSongScene")
        {
            currentLyricsIndex = mournfulLyricsList[Random.Range(0, mournfulLyricsList.Count)];
        }
        if (sceneName == "WarSongScene")
        {
            currentLyricsIndex = militaryLyricsList[Random.Range(0, militaryLyricsList.Count)];
        }
        if (sceneName == "WisdomSongScene")
        {
            currentLyricsIndex = partyLyricsList[Random.Range(0, partyLyricsList.Count)];
        }

        changeLyrics();
    }

    // Update is called once per frame
    void Update()
    {
		Color lyricsColor = lyricsText.color;  //  sets color to object
        lyricsColor.a = GameManager.currentScore / GameManager.targetScore;// changes the color of alpha
        lyricsText.color = lyricsColor;
       // Debug.Log(lyricsColor.a);
       // GameManager.ChangeOppacityOfLyrics(lyricsText);
    }

    public void changeLyrics()
    {
        //using a game object 
        //changingLyrics.GetComponent<Text>().text = currentLyricsIndex; 

        //using a Text 
        lyricsText.text = currentLyricsIndex;
    }
}
