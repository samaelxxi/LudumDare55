using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Oinks
{
    public static List<string> default1Oinks = new()
    { "smallOink", "smallOink2", "smallOink3" };

    public static List<string> default2Oinks = new()
    { "smallOink", "smallOink3", "smallOink34" };

    public static List<string> default3Oinks = new()
    { "smallOink2", "smallOink34", "smallOink35" };

    public static List<string> fat1Oinks = new()
    { "fat1Oink1", "fat1Oink2", "fat2Oink1" };

    public static List<string> fat2Oinks = new()
    { "fat2Oink1", "fat2Oink2", "fat3Oink1" };

    public static List<string> fat3Oinks = new()
    { "fat3Oink1", "fat3Oink2", "fat2Oink2" };

    public static List<string> fast1Oinks = new()
    { "fast1Oink1", "fast1Oink2", "fast2Oink1" };

    public static List<string> fast2Oinks = new()
    { "fast2Oink1", "fast2Oink2", "fast3Oink1" };

    public static List<string> fast3Oinks = new()
    { "fast3Oink1", "fast3Oink2", "fast2Oink2" };

    public static List<string> defaultUpgrades = new()
    { null, "defaultUpgrade2", "defaultUpgrade3" };

    public static List<string> fatUpgrades = new()
    { "fatUpgrade1", "fatUpgrade2", "fatUpgrade3" };

    public static List<string> fastUpgrades = new()
    { "fastUpgrade1", "fastUpgrade2", "fastUpgrade3" };

    public static List<string> GetOinks(PiggyType type, int rank)
    {
        switch (type)
        {
            case PiggyType.Normal:
                return rank switch
                {
                    1 => default1Oinks,
                    2 => default2Oinks,
                    3 => default3Oinks,
                    _ => default1Oinks
                };
            case PiggyType.Fat:
                return rank switch
                {
                    1 => fat1Oinks,
                    2 => fat2Oinks,
                    3 => fat3Oinks,
                    _ => fat1Oinks
                };
            case PiggyType.Fast:
                return rank switch
                {
                    1 => fast1Oinks,
                    2 => fast2Oinks,
                    3 => fast3Oinks,
                    _ => fast1Oinks
                };
            default:
                return default1Oinks;
        }
    }

    public static string GetUpgradeOink(PiggyType type, int rank)
    {
        switch (type)
        {
            case PiggyType.Normal:
                return defaultUpgrades[rank];
            case PiggyType.Fat:
                return fatUpgrades[rank];
            case PiggyType.Fast:
                return fastUpgrades[rank];
            default:
                return defaultUpgrades[rank];
        }
    }
}
