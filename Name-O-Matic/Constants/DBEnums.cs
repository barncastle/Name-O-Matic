namespace NameOMatic.Constants
{
    sealed class TextureSuffix
    {
        public static string Get(int value)
        {
            switch (value)
            {
                case 1: return "_s";
                case 2: return "_e";
                default: return "";
            }
        }
    }

    public enum ComponentGender : byte
    {
        M = 0,
        F = 1,
        O = 2,
        U = 3,
    }

    public enum ComponentSection : byte
    {
        AU = 0, // ArmUpper
        AL = 1, // ArmLower
        HA = 2, // Hand
        TU = 3, // TorsoUpper
        TL = 4, // TorsoLower
        LU = 5, // LegUpper
        LL = 6, // LegLower
        FO = 7, // Foot
        PR = 8, // Accessory
    }

    public enum ComponentTier : byte
    {
        Common,
        Rare,
        Epic
    }

    public enum CharSection
    {
        Skin = 0,
        Face = 1,
        FacialHair = 2,
        Hair = 3,
        Underwear = 4,
        HDSkin = 5,
        HDFace = 6,
        HDFacialHair = 7,
        HDHair = 8,
        HDUnderwear = 9,
        Custom1 = 10,
        HDCustom1 = 11,
        Custom2 = 12,
        HDCustom2 = 13,
        Custom3 = 14,
        HDCustom3 = 15
    }

    public enum VocalUISound
    {
        INVENTORYFULL = 0,
        OUTOFAMMO = 1,
        NOEQUIP_LEVEL = 2,
        NOEQUIP_EVER = 3,
        BOUND_NODROP = 4,
        ITEMCOOLING = 5,
        CANTDRINKMORE = 6,
        CANTEATMORE = 7,
        CANTINVITE = 8,
        INVITEEBUSY = 9,
        TARGETTOOFAR = 10,
        INVALIDTARGET = 11,
        SPELLCOOLING = 12,
        CANTLEARN_LEVEL = 13,
        LOCKED = 14,
        NOMANA = 15,
        NOTWHILEDEAD = 16,
        CANTLOOT = 17,
        CANTCREATE = 18,
        DECLINEGROUP = 19,
        ALREADYINGROUP = 20,
        ALREADYINGUILD = 21,
        CANTAFFORDBANKSLOT = 22,
        TOOMANYBANKSLOTS = 23,
        CANTEAT_MOVING = 24,
        NOTABAG = 25,
        CANTPUTBAG = 26,
        WRONGSLOT = 27,
        AMMOONLYINBAG = 28,
        BAGFULL = 29,
        ITEMMAXCOUNT = 30,
        CANTLOOT_DIDNTKILL = 31,
        CANTLOOT_WRONGFACING = 32,
        CANTLOOT_LOCKED = 33,
        CANTLOOT_NOTSTANDING = 34,
        CANTLOOT_TOOFAR = 35,
        CANTATTACKRONGDIRECTION = 36,
        CANTATTACK_NOTSTANDING = 37,
        CANTATTACK_NOTARGET = 38,
        NOTENOUGHGOLD = 39,
        NOTENOUGHMONEY = 40,
        CANTEQUIP2H_SKILL = 41,
        CANTEQUIP_2HEQUIPPED = 42,
        CANTEQUIP2H_NOSKILL = 43,
        NOTEQUIPPABLE = 44,
        GENERICNOTARGET = 45,
        CANTCAST_OUTOFRANGE = 46,
        POTIONCOOLING = 47,
        PROFICIENCYNEEDED = 48,
        MUSTEQUIPPITEM = 49,
        ABILITYCOOLING = 50,
        CANTUSEITEM = 51,
        CHESTINUSE = 52,
        FOODCOOLING = 53,
        CANTTAXI_NOMONEY = 54,
        CANTUSELOCKED = 55,
        NOEQUIPSLOTAVAILABLE = 56,
        CANTUSETOOFAR = 57,
        CANTSWAP = 58,
        CANTTRADE_SOULBOUND = 59,
        NOTOWNER = 60,
        ITEMLOCKED = 61,
        GUILDPERMISSIONS = 62,
        NORAGE = 63,
        NOENERGY = 64,
        NOFOCUS = 65,
    }
}
