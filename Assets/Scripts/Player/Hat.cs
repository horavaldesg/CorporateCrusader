using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class Hat : ScriptableObject
{
    public enum ChosenHat
    {
        Construction,
        Chef,
        Policeman,
        Fireman,
        Nurse,
        Cowboy,
        TopHat,
        Pilot,
        Astronaut,
        Fisherman,
        Grad,
        Sombrero,
        Detective,
        Safari,
        Farmer,
        Army,
        Trucker,
        King,
        Artist,
        Pirate,
        Football,
        Miner,
        Sailor,
        Welder,
        LumberJack,
        RaceCar,
        Florist,
        Viking,
        Knight,
        Jester
    };

    public ChosenHat chosenHat;

    public List<ChosenHat> chosenHats = new ();

    public Dictionary<ChosenHat, object[]> hatContainer = new();

  
    /*
    public ChosenHat hat1;
    public ChosenHat hat2;
    public ChosenHat hat3;
    public ChosenHat hat4;
    public ChosenHat hat5;
    public ChosenHat hat6;*/
}
