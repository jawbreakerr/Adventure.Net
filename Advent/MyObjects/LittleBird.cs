using System;
using Adventure.Net;
using Adventure.Net.Verbs;
using Object = Adventure.Net.Object;

namespace ColossalCave.MyObjects
{
    public class LittleBird : Object
    {
        public override void Initialize()
        {
            Name = "little bird";
            Synonyms.Are("cheerful", "mournful", "little", "bird");
            IsAnimate = true;

            Before<Release>(() =>
                {
                    var cage = Objects.Get<WickerCage>();
                    var bird = Objects.Get<LittleBird>();

                    if (!cage.Contents.Contains(bird))
                    {
                        Print("The bird is not caged now.");
                        return true;
                    }
                    else
                    {
                        cage.IsOpen = true;
                        cage.Remove(bird);
                        bird.MoveToLocation();
                        return false;
                    }
                });
        }
    }

//Object  -> little_bird "little bird"
//with  name 'cheerful' 'mournful' 'little' 'bird',
//    initial "A cheerful little bird is sitting here singing.",
//    before [;
//      Examine:
//        if (self in wicker_cage)
//            "The little bird looks unhappy in the cage.";
//        "The cheerful little bird is sitting here singing.";
//      Insert:
//        if (second == wicker_cage)
//            <<Catch self>>;
//        else
//            "Don't put the poor bird in ", (the) second, "!";
//      Drop, Remove:
//        if (self in wicker_cage) {
//            print "(The bird is released from the cage.)^^";
//            <<Release self>>;
//        }
//      Take, Catch:
//        if (self in wicker_cage)
//            "You already have the little bird.
//             If you take it out of the cage it will likely fly away from you.";
//        if (wicker_cage notin player)
//            "You can catch the bird, but you cannot carry it.";
//        if (black_rod in player)
//            "The bird was unafraid when you entered,
//             but as you approach it becomes disturbed and you cannot catch it.";
//        move self to wicker_cage;
//        give wicker_cage ~open;
//        "You catch the bird in the wicker cage.";
//      Release:
//        if (self notin wicker_cage)
//            "The bird is not caged now.";
//        give wicker_cage open;
//        move self to location;
//        if (Snake in location) {
//            remove Snake;
//            "The little bird attacks the green snake,
//             and in an astounding flurry drives the snake away.";
//        }
//        if (Dragon in location) {
//            remove self;
//            "The little bird attacks the green dragon,
//             and in an astounding flurry gets burnt to a cinder.
//             The ashes blow away.";
//        }
//        "The little bird flies free.";
//    ],
//    life [;
//      Give:
//        "It's not hungry. (It's merely pinin' for the fjords).
//         Besides, I suspect it would prefer bird seed.";
//      Order, Ask, Answer:
//        "Cheep! Chirp!";
//      Attack:
//        if (self in wicker_cage)
//            "Oh, leave the poor unhappy bird alone.";
//        remove self;
//        "The little bird is now dead. Its body disappears.";
//    ],
//has   animate;

}
