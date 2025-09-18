using CarLookup.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarLookup.Access.Seed;

/// <summary>
/// Static class containing seed data for the application
/// This provides initial car makes and car models for development and demo purposes
/// </summary>
public static class SeedData
{
    // Define the makes with stable GUIDs and countries
    private static readonly (string Id, string Name, string Country)[] MakeDefinitions =
    {
        ("f47ac10b-58cc-4372-a567-0e02b2c3d479", "Toyota", "Japan"),
        ("f47ac10b-58cc-4372-a567-0e02b2c3d480", "Honda", "Japan"),
        ("f47ac10b-58cc-4372-a567-0e02b2c3d481", "Ford", "United States"),
        ("f47ac10b-58cc-4372-a567-0e02b2c3d482", "BMW", "Germany"),
        ("f47ac10b-58cc-4372-a567-0e02b2c3d483", "Mercedes-Benz", "Germany"),
        ("a47ac10b-58cc-4372-a567-0e02b2c3d484", "Volkswagen", "Germany"),
        ("b47ac10b-58cc-4372-a567-0e02b2c3d485", "Chevrolet", "United States"),
        ("c47ac10b-58cc-4372-a567-0e02b2c3d486", "Nissan", "Japan"),
        ("d47ac10b-58cc-4372-a567-0e02b2c3d487", "Hyundai", "South Korea"),
        ("e47ac10b-58cc-4372-a567-0e02b2c3d488", "Kia", "South Korea"),
        ("f57ac10b-58cc-4372-a567-0e02b2c3d489", "Audi", "Germany"),
        ("067ac10b-58cc-4372-a567-0e02b2c3d48a", "Peugeot", "France"),
        ("177ac10b-58cc-4372-a567-0e02b2c3d48b", "Renault", "France"),
        ("287ac10b-58cc-4372-a567-0e02b2c3d48c", "Fiat", "Italy"),
        ("397ac10b-58cc-4372-a567-0e02b2c3d48d", "Subaru", "Japan"),
        ("4a7ac10b-58cc-4372-a567-0e02b2c3d48e", "Mazda", "Japan"),
        ("5b7ac10b-58cc-4372-a567-0e02b2c3d48f", "Tesla", "United States"),
        ("6c7ac10b-58cc-4372-a567-0e02b2c3d490", "Volvo", "Sweden"),
        ("7d7ac10b-58cc-4372-a567-0e02b2c3d491", "Mitsubishi", "Japan"),
        ("8e7ac10b-58cc-4372-a567-0e02b2c3d492", "Land Rover", "United Kingdom"),
        ("9f7ac10b-58cc-4372-a567-0e02b2c3d493", "Jaguar", "United Kingdom"),
        ("a07ac10b-58cc-4372-a567-0e02b2c3d494", "Porsche", "Germany"),
        ("b17ac10b-58cc-4372-a567-0e02b2c3d495", "Lexus", "Japan"),
        ("c27ac10b-58cc-4372-a567-0e02b2c3d496", "Acura", "Japan"),
        ("d37ac10b-58cc-4372-a567-0e02b2c3d497", "Infiniti", "Japan")
    };

    /// <summary>
    /// Mapping of makes to their car models with years
    /// </summary>
    private static readonly Dictionary<string, (string Model, int Year)[]> ModelsByMake = new()
    {
        ["Toyota"] =
        [
            ("Camry", 2023), ("Corolla", 2023), ("Prius", 2023), ("RAV4", 2022), ("Highlander", 2023),
            ("Sienna", 2022), ("Tacoma", 2023), ("4Runner", 2022), ("Tundra", 2023), ("Sequoia", 2023),
            ("Avalon", 2022), ("C-HR", 2023), ("Venza", 2022), ("Supra", 2023), ("86", 2022),
            ("Yaris", 2020), ("Prius Prime", 2023), ("Mirai", 2022), ("Land Cruiser", 2021), ("GR Corolla", 2023),
            ("bZ4X", 2023), ("Crown", 2023), ("Prius C", 2019), ("Matrix", 2014), ("FJ Cruiser", 2014),
            ("Celica", 2006), ("MR2", 2005), ("Tercel", 1999), ("Echo", 2005), ("Solara", 2008),
            ("Previa", 1997), ("T100", 1998), ("Pickup", 1995), ("Van", 1989), ("Cressida", 1992),
            ("Scion tC", 2016), ("Scion xB", 2015), ("Scion xD", 2014), ("Scion FR-S", 2016), ("Scion iQ", 2015)
        ],
        ["Honda"] =
        [
            ("Civic", 2023), ("Accord", 2023), ("CR-V", 2022), ("Pilot", 2023), ("Odyssey", 2022),
            ("Ridgeline", 2023), ("HR-V", 2023), ("Passport", 2022), ("Insight", 2022), ("Fit", 2020),
            ("Clarity", 2021), ("Element", 2011), ("S2000", 2009), ("NSX", 2022), ("Prelude", 2001),
            ("CRX", 1991), ("Del Sol", 1997), ("Crosstour", 2015), ("CR-Z", 2016), ("Civic Si", 2023),
            ("Civic Type R", 2023), ("Accord Hybrid", 2023), ("CR-V Hybrid", 2023), ("Pilot Elite", 2023), ("Ridgeline RTL-E", 2023),
            ("Odyssey Touring", 2023), ("Passport Elite", 2023), ("HR-V Sport", 2023), ("Insight Touring", 2022), ("Civic Hatchback", 2023),
            ("Civic Sedan", 2023), ("Civic Coupe", 2020), ("Accord Sport", 2023), ("Accord EX-L", 2023), ("Pilot Touring", 2023)
        ],
        ["Ford"] =
        [
            ("F-150", 2023), ("Mustang", 2023), ("Explorer", 2022), ("Escape", 2023), ("Edge", 2022),
            ("Bronco", 2023), ("Expedition", 2023), ("Transit", 2023), ("Ranger", 2023), ("Maverick", 2023),
            ("EcoSport", 2022), ("Fusion", 2020), ("Fiesta", 2019), ("Focus", 2018), ("Taurus", 2019),
            ("Flex", 2019), ("C-Max", 2018), ("GT", 2022), ("Bronco Sport", 2023), ("F-250", 2023),
            ("F-350", 2023), ("F-450", 2023), ("E-Series", 2023), ("Transit Connect", 2023), ("Mustang Mach-E", 2023),
            ("Lightning", 2023), ("Raptor", 2023), ("King Ranch", 2023), ("Platinum", 2023), ("Limited", 2023),
            ("ST", 2019), ("RS", 2018), ("SHO", 2019), ("SVT", 2014), ("Shelby", 2023)
        ],
        ["BMW"] =
        [
            ("3 Series", 2023), ("5 Series", 2023), ("7 Series", 2023), ("X3", 2022), ("X5", 2022),
            ("X1", 2023), ("X2", 2023), ("X4", 2023), ("X6", 2023), ("X7", 2023),
            ("Z4", 2023), ("8 Series", 2023), ("4 Series", 2023), ("2 Series", 2023), ("i3", 2022),
            ("i4", 2023), ("iX", 2023), ("i7", 2023), ("iX3", 2023), ("M3", 2023),
            ("M5", 2023), ("M8", 2023), ("X3 M", 2023), ("X5 M", 2023), ("X6 M", 2023),
            ("330i", 2023), ("340i", 2023), ("M340i", 2023), ("530i", 2023), ("540i", 2023),
            ("M550i", 2023), ("740i", 2023), ("750i", 2023), ("M760i", 2023), ("840i", 2023)
        ],
        ["Mercedes-Benz"] =
        [
            ("C-Class", 2023), ("E-Class", 2023), ("S-Class", 2022), ("GLC", 2023), ("GLE", 2022),
            ("GLS", 2023), ("GLA", 2023), ("GLB", 2023), ("G-Class", 2023), ("A-Class", 2022),
            ("CLA", 2023), ("CLS", 2023), ("SL", 2023), ("AMG GT", 2023), ("Maybach S-Class", 2023),
            ("EQS", 2023), ("EQE", 2023), ("EQB", 2023), ("EQA", 2023), ("EQC", 2022),
            ("C63 AMG", 2023), ("E63 AMG", 2023), ("S63 AMG", 2023), ("GLC63 AMG", 2023), ("GLE63 AMG", 2023),
            ("C300", 2023), ("C43 AMG", 2023), ("E350", 2023), ("E450", 2023), ("S450", 2023),
            ("S500", 2023), ("GLC300", 2023), ("GLE350", 2023), ("GLE450", 2023), ("GLS450", 2023)
        ],
        ["Volkswagen"] =
        [
            ("Golf", 2023), ("Passat", 2022), ("Tiguan", 2023), ("Jetta", 2023), ("Atlas", 2023),
            ("Arteon", 2023), ("Taos", 2023), ("ID.4", 2023), ("Beetle", 2019), ("CC", 2017),
            ("Touareg", 2023), ("Phaeton", 2016), ("Eos", 2016), ("Routan", 2014), ("Rabbit", 2009),
            ("Golf R", 2023), ("GTI", 2023), ("GLI", 2023), ("Golf Alltrack", 2019), ("Golf SportWagen", 2019),
            ("Passat Alltrack", 2019), ("Atlas Cross Sport", 2023), ("Tiguan Allspace", 2023), ("T-Cross", 2023), ("T-Roc", 2023),
            ("Polo", 2023), ("Up!", 2023), ("Amarok", 2023), ("Crafter", 2023), ("Caddy", 2023),
            ("Transporter", 2023), ("Multivan", 2023), ("Caravelle", 2023), ("California", 2023), ("Grand California", 2023)
        ],
        ["Chevrolet"] =
        [
            ("Silverado", 2023), ("Malibu", 2022), ("Equinox", 2023), ("Camaro", 2023), ("Tahoe", 2022),
            ("Suburban", 2023), ("Traverse", 2023), ("Blazer", 2023), ("Colorado", 2023), ("Corvette", 2023),
            ("Impala", 2020), ("Cruze", 2019), ("Sonic", 2020), ("Spark", 2022), ("Bolt EV", 2023),
            ("Bolt EUV", 2023), ("Express", 2023), ("Silverado HD", 2023), ("Trailblazer", 2023), ("Trax", 2024),
            ("SS", 2017), ("Volt", 2019), ("Captiva", 2016), ("Avalanche", 2013), ("HHR", 2011),
            ("Cobalt", 2010), ("Aveo", 2011), ("Monte Carlo", 2007), ("SSR", 2006), ("Tracker", 2004),
            ("S-10", 2004), ("Astro", 2005), ("Venture", 2005), ("Prizm", 2002), ("Metro", 2001)
        ],
        ["Nissan"] =
        [
            ("Altima", 2023), ("Sentra", 2022), ("Rogue", 2023), ("Maxima", 2022), ("Pathfinder", 2023),
            ("Murano", 2023), ("Armada", 2023), ("Titan", 2023), ("Frontier", 2023), ("Kicks", 2023),
            ("Versa", 2023), ("370Z", 2020), ("GT-R", 2023), ("Leaf", 2023), ("Ariya", 2023),
            ("NV200", 2023), ("NV Cargo", 2023), ("Juke", 2017), ("Cube", 2014), ("Xterra", 2015),
            ("Quest", 2017), ("Rogue Sport", 2022), ("Pathfinder Rock Creek", 2023), ("Titan XD", 2019), ("Note", 2019),
            ("Micra", 2023), ("Qashqai", 2023), ("X-Trail", 2023), ("Navara", 2023), ("Patrol", 2023),
            ("370Z Nismo", 2020), ("GT-R Nismo", 2023), ("Sentra SR", 2023), ("Altima SR", 2023), ("Rogue SL", 2023)
        ],
        ["Hyundai"] =
        [
            ("Elantra", 2023), ("Sonata", 2022), ("Tucson", 2023), ("Santa Fe", 2022), ("Palisade", 2023),
            ("Kona", 2023), ("Venue", 2023), ("Ioniq 5", 2023), ("Ioniq 6", 2023), ("Genesis", 2016),
            ("Veloster", 2022), ("Accent", 2022), ("Santa Cruz", 2023), ("Nexo", 2023), ("Azera", 2017),
            ("Entourage", 2009), ("Veracruz", 2012), ("Tiburon", 2008), ("XG", 2005), ("Scoupe", 1995),
            ("Elantra GT", 2020), ("Elantra N", 2023), ("Sonata N Line", 2023), ("Tucson N Line", 2023), ("Santa Fe N Line", 2023),
            ("Kona Electric", 2023), ("Kona N", 2023), ("Veloster N", 2022), ("i10", 2023), ("i20", 2023),
            ("i30", 2023), ("Bayon", 2023), ("Santa Fe XL", 2018), ("Genesis Coupe", 2016), ("Equus", 2016)
        ],
        ["Kia"] =
        [
            ("Optima", 2022), ("Soul", 2023), ("Sportage", 2023), ("Sorento", 2022), ("Telluride", 2023),
            ("Forte", 2023), ("Rio", 2022), ("Seltos", 2023), ("Niro", 2023), ("Stinger", 2023),
            ("Carnival", 2023), ("EV6", 2023), ("Cadenza", 2020), ("Amanti", 2009), ("Spectra", 2009),
            ("Rondo", 2012), ("Borrego", 2011), ("Sedona", 2021), ("K900", 2020), ("Magentis", 2010),
            ("Forte5", 2019), ("Soul EV", 2023), ("Niro EV", 2023), ("Niro Hybrid", 2023), ("Sorento Hybrid", 2023),
            ("Sportage Hybrid", 2023), ("Optima Hybrid", 2020), ("Picanto", 2023), ("Ceed", 2023), ("XCeed", 2023),
            ("ProCeed", 2023), ("Cerato", 2023), ("K5", 2023), ("K3", 2023), ("Carens", 2023)
        ],
        ["Audi"] =
        [
            ("A4", 2023), ("A6", 2022), ("Q5", 2023), ("Q7", 2022), ("A3", 2023),
            ("A8", 2023), ("Q3", 2023), ("Q8", 2023), ("TT", 2023), ("R8", 2023),
            ("e-tron", 2023), ("e-tron GT", 2023), ("A5", 2023), ("A7", 2023), ("S3", 2023),
            ("S4", 2023), ("S5", 2023), ("S6", 2023), ("S7", 2023), ("S8", 2023),
            ("RS3", 2023), ("RS4", 2023), ("RS5", 2023), ("RS6", 2023), ("RS7", 2023),
            ("RS Q8", 2023), ("SQ5", 2023), ("SQ7", 2023), ("SQ8", 2023), ("TTS", 2023),
            ("RS e-tron GT", 2023), ("Q4 e-tron", 2023), ("allroad", 2020), ("Cabriolet", 2009), ("Coupe", 1996)
        ],
        ["Peugeot"] =
        [
            ("208", 2023), ("308", 2022), ("3008", 2023), ("5008", 2023), ("2008", 2023),
            ("508", 2023), ("Partner", 2023), ("Expert", 2023), ("Boxer", 2023), ("Rifter", 2023),
            ("Traveller", 2023), ("e-208", 2023), ("e-2008", 2023), ("e-Expert", 2023), ("e-Boxer", 2023),
            ("108", 2022), ("301", 2023), ("408", 2023), ("4008", 2018), ("1007", 2009),
            ("107", 2014), ("206", 2012), ("207", 2014), ("307", 2008), ("407", 2011),
            ("607", 2010), ("RCZ", 2015), ("Bipper", 2017), ("807", 2014), ("Ion", 2009)
        ],
        ["Renault"] =
        [
            ("Clio", 2023), ("Megane", 2022), ("Captur", 2023), ("Kadjar", 2022), ("Koleos", 2023),
            ("Scenic", 2023), ("Espace", 2023), ("Talisman", 2023), ("Twingo", 2023), ("Zoe", 2023),
            ("Master", 2023), ("Trafic", 2023), ("Kangoo", 2023), ("Arkana", 2023), ("Austral", 2023),
            ("Duster", 2023), ("Logan", 2023), ("Sandero", 2023), ("Symbol", 2023), ("Latitude", 2015),
            ("Laguna", 2015), ("Vel Satis", 2009), ("Avantime", 2003), ("Wind", 2012), ("Fluence", 2016),
            ("Megane RS", 2020), ("Clio RS", 2018), ("Alpine A110", 2023), ("Alpine A290", 2024), ("Kwid", 2023)
        ],
        ["Fiat"] =
        [
            ("500", 2023), ("Panda", 2022), ("Tipo", 2023), ("500X", 2023), ("500L", 2020),
            ("Punto", 2018), ("Bravo", 2014), ("Linea", 2016), ("Freemont", 2016), ("Croma", 2011),
            ("Multipla", 2010), ("Stilo", 2007), ("Marea", 2007), ("Barchetta", 2005), ("Coupe", 2000),
            ("Uno", 2013), ("Palio", 2017), ("Siena", 2017), ("Strada", 2023), ("Toro", 2023),
            ("Doblo", 2023), ("Ducato", 2023), ("Talento", 2023), ("Fullback", 2018), ("124 Spider", 2020),
            ("500e", 2023), ("500 Abarth", 2019), ("595 Abarth", 2023), ("695 Abarth", 2023), ("Panda 4x4", 2023)
        ],
        ["Subaru"] =
        [
            ("Impreza", 2023), ("Forester", 2022), ("Outback", 2023), ("Crosstrek", 2022), ("Legacy", 2023),
            ("Ascent", 2023), ("BRZ", 2023), ("WRX", 2023), ("STI", 2021), ("Baja", 2006),
            ("Tribeca", 2014), ("SVX", 1997), ("XT", 1991), ("Justy", 1994), ("Loyale", 1994),
            ("GL", 1994), ("DL", 1989), ("Leone", 1994), ("Impreza WRX", 2023), ("Forester XT", 2018),
            ("Outback XT", 2023), ("Legacy GT", 2009), ("Crosstrek Sport", 2023), ("Wilderness", 2023), ("Touring", 2023),
            ("Premium", 2023), ("Limited", 2023), ("Sport", 2023), ("Base", 2023), ("Convenience", 2023)
        ],
        ["Mazda"] =
        [
            ("Mazda3", 2023), ("Mazda6", 2022), ("CX-5", 2023), ("CX-9", 2022), ("CX-30", 2023),
            ("CX-50", 2023), ("MX-5 Miata", 2023), ("CX-3", 2019), ("Mazda2", 2020), ("Mazda5", 2015),
            ("RX-8", 2011), ("RX-7", 1995), ("Millenia", 2002), ("Protege", 2003), ("B-Series", 2009),
            ("Tribute", 2011), ("MPV", 2006), ("323", 1994), ("626", 2002), ("929", 1995),
            ("MX-3", 1995), ("MX-6", 1997), ("Navajo", 1994), ("Truck", 1993), ("CX-60", 2023),
            ("CX-70", 2024), ("CX-90", 2023), ("Mazda3 Turbo", 2023), ("CX-5 Turbo", 2023), ("MX-30", 2022)
        ],
        ["Tesla"] =
        [
            ("Model S", 2023), ("Model 3", 2023), ("Model X", 2022), ("Model Y", 2023), ("Cybertruck", 2024),
            ("Roadster", 2020), ("Semi", 2023), ("Model S Plaid", 2023), ("Model X Plaid", 2023), ("Model 3 Performance", 2023),
            ("Model Y Performance", 2023), ("Model S Long Range", 2023), ("Model 3 Long Range", 2023), ("Model Y Long Range", 2023), ("Model 3 Standard Range", 2022),
            ("Model S P100D", 2019), ("Model X P100D", 2019), ("Model 3 P", 2021), ("Model Y P", 2021), ("Model S 75D", 2019),
            ("Model S 85D", 2016), ("Model S P85D", 2016), ("Model X 75D", 2019), ("Model X 90D", 2019), ("Model X P90D", 2019),
            ("Model 3 RWD", 2023), ("Model Y RWD", 2022), ("Model S Refresh", 2021), ("Model X Refresh", 2021), ("Original Roadster", 2012)
        ],
        ["Volvo"] =
        [
            ("S60", 2023), ("V60", 2022), ("XC60", 2023), ("XC90", 2022), ("S90", 2023),
            ("V90", 2023), ("XC40", 2023), ("C40", 2023), ("EX30", 2024), ("EX90", 2024),
            ("V40", 2019), ("S40", 2011), ("C30", 2013), ("C70", 2013), ("S80", 2016),
            ("V70", 2016), ("XC70", 2016), ("240", 1993), ("740", 1992), ("940", 1998),
            ("850", 1997), ("960", 1997), ("780", 1991), ("760", 1990), ("Amazon", 1970),
            ("Polestar 1", 2021), ("Polestar 2", 2023), ("S60 Polestar", 2018), ("V60 Polestar", 2018), ("XC90 Excellence", 2023)
        ],
        ["Mitsubishi"] =
        [
            ("Outlander", 2023), ("Eclipse Cross", 2023), ("Mirage", 2023), ("Lancer", 2017), ("Pajero", 2021),
            ("ASX", 2023), ("L200", 2023), ("Shogun", 2019), ("Colt", 2012), ("Carisma", 2004),
            ("Galant", 2012), ("Grandis", 2011), ("Spacewagon", 2004), ("3000GT", 1999), ("Starion", 1989),
            ("Eclipse", 2012), ("Endeavor", 2011), ("Montero", 2006), ("Montero Sport", 2004), ("Diamante", 2004),
            ("Mirage G4", 2023), ("Outlander PHEV", 2023), ("i-MiEV", 2017), ("Raider", 2009), ("Outlander Sport", 2020),
            ("Lancer Evolution", 2015), ("FTO", 2000), ("GTO", 1999), ("Sigma", 1996), ("Cordia", 1988)
        ],
        ["Land Rover"] =
        [
            ("Defender", 2023), ("Discovery", 2022), ("Range Rover", 2023), ("Range Rover Sport", 2023), ("Range Rover Evoque", 2023),
            ("Range Rover Velar", 2023), ("Discovery Sport", 2023), ("Freelander", 2014), ("LR2", 2015), ("LR3", 2009),
            ("LR4", 2016), ("Series I", 1958), ("Series II", 1971), ("Series III", 1985), ("Range Rover Classic", 1996),
            ("Discovery 1", 1998), ("Discovery 2", 2004), ("Defender 90", 2023), ("Defender 110", 2023), ("Defender 130", 2023),
            ("Range Rover Autobiography", 2023), ("Range Rover SVR", 2023), ("Range Rover P400e", 2023), ("Discovery HSE", 2023), ("Evoque Convertible", 2019),
            ("Velar P400", 2023), ("Velar SVR", 2021), ("Discovery Commercial", 2023), ("Defender Commercial", 2023), ("Range Rover Commercial", 2023)
        ],
        ["Jaguar"] =
        [
            ("XE", 2022), ("XF", 2023), ("F-Pace", 2023), ("E-Pace", 2023), ("I-Pace", 2023),
            ("F-Type", 2023), ("XJ", 2019), ("S-Type", 2008), ("X-Type", 2009), ("XK", 2015),
            ("XKR", 2015), ("XJR", 2019), ("XFR", 2015), ("XE SV Project 8", 2019), ("F-Type SVR", 2023),
            ("F-Pace SVR", 2023), ("XF Sportbrake", 2019), ("Mark 2", 1967), ("E-Type", 1975), ("XJS", 1996),
            ("Vanden Plas", 1990), ("Sovereign", 1997), ("Super V8", 2003), ("Portfolio", 2012), ("R", 2015),
            ("R-S", 2015), ("Checkered Flag", 2019), ("Project 7", 2015), ("Project 8", 2018), ("Heritage Edition", 2021)
        ],
        ["Porsche"] =
        [
            ("911", 2023), ("Cayenne", 2022), ("Macan", 2023), ("Panamera", 2022), ("Taycan", 2023),
            ("718 Boxster", 2023), ("718 Cayman", 2023), ("Boxster", 2016), ("Cayman", 2016), ("Carrera GT", 2006),
            ("959", 1988), ("928", 1995), ("924", 1988), ("944", 1991), ("968", 1995),
            ("356", 1965), ("550 Spyder", 1955), ("Cayenne Turbo", 2023), ("Macan Turbo", 2023), ("Panamera Turbo", 2023),
            ("911 Turbo", 2023), ("911 GT3", 2023), ("911 GT2", 2019), ("718 Spyder", 2023), ("718 GT4", 2023),
            ("Taycan Turbo", 2023), ("Taycan Cross Turismo", 2023), ("Cayenne Coupe", 2023), ("Macan S", 2023), ("Panamera Sport Turismo", 2023)
        ],
        ["Lexus"] =
        [
            ("ES", 2023), ("IS", 2022), ("RX", 2023), ("NX", 2023), ("GX", 2023),
            ("LX", 2023), ("UX", 2023), ("LC", 2023), ("LS", 2023), ("RC", 2023),
            ("CT", 2017), ("HS", 2012), ("SC", 2010), ("GS", 2020), ("LFA", 2012),
            ("IS F", 2014), ("GS F", 2020), ("RC F", 2023), ("LC F", 2022), ("LS F", 2024),
            ("RX L", 2023), ("GX 460", 2023), ("LX 600", 2023), ("ES 350", 2023), ("IS 350", 2023),
            ("RX 350", 2023), ("NX 350", 2023), ("UX 250h", 2023), ("LC 500", 2023), ("LS 500", 2023)
        ],
        ["Acura"] =
        [
            ("TLX", 2023), ("RDX", 2022), ("MDX", 2023), ("ILX", 2022), ("NSX", 2022),
            ("TSX", 2014), ("TL", 2014), ("RL", 2012), ("ZDX", 2013), ("RSX", 2006),
            ("Integra", 2023), ("Legend", 1995), ("Vigor", 1994), ("CL", 2003), ("EL", 2005),
            ("SLX", 1999), ("CSX", 2011), ("RLX", 2020), ("TLX Type S", 2023), ("MDX Type S", 2023),
            ("RDX A-Spec", 2023), ("ILX Premium", 2022), ("NSX Type S", 2022), ("Integra Type S", 2023), ("TLX PMC Edition", 2021),
            ("MDX PMC Edition", 2020), ("RDX PMC Edition", 2019), ("NSX PMC Edition", 2019), ("TL Type S", 2014), ("TSX Sport Wagon", 2014)
        ],
        ["Infiniti"] =
        [
            ("Q50", 2022), ("QX50", 2023), ("QX60", 2022), ("QX80", 2023), ("Q60", 2022),
            ("Q70", 2019), ("QX70", 2017), ("Q40", 2015), ("QX56", 2013), ("M", 2013),
            ("FX", 2013), ("EX", 2013), ("G", 2013), ("JX", 2013), ("I", 2004),
            ("J30", 1997), ("Q45", 2006), ("QX4", 2003), ("I30", 2001), ("I35", 2004),
            ("Q50 Red Sport", 2023), ("Q60 Red Sport", 2022), ("QX50 Sensory", 2023), ("QX60 Autograph", 2023), ("QX80 Limited", 2023),
            ("Q50 Hybrid", 2019), ("M35h", 2013), ("M37", 2013), ("M56", 2013), ("FX35", 2013)
        ]
    };

    /// <summary>
    /// Get initial car makes for seeding
    /// These are well-known car manufacturers with stable GUIDs for consistent seeding
    /// </summary>
    /// <returns>Collection of CarMake entities with predefined IDs and timestamps</returns>
    public static IEnumerable<CarMake> GetCarMakes()
    {
        var now = DateTime.UtcNow;

        return MakeDefinitions.Select(md => new CarMake
        {
            MakeId = Guid.Parse(md.Id),
            Name = md.Name,
            CountryOfOrigin = md.Country,
            CreatedAt = now
        }).ToList();
    }

    /// <summary>
    /// Get initial car models for seeding
    /// </summary>
    /// <returns>Collection of CarModel entities distributed across all makes</returns>
    public static IEnumerable<CarModel> GetCarModels()
    {
        var now = DateTime.UtcNow;
        var makeIdLookup = MakeDefinitions.ToDictionary(md => md.Name, md => Guid.Parse(md.Id));
        var models = new List<CarModel>();

        foreach (var (makeName, modelData) in ModelsByMake)
        {
            if (!makeIdLookup.TryGetValue(makeName, out var makeId))
                continue;

            foreach (var (modelName, year) in modelData)
            {
                models.Add(new CarModel
                {
                    ModelId = Guid.NewGuid(),
                    Name = modelName,
                    MakeId = makeId,
                    ModelYear = year,
                    CreatedAt = now
                });
            }
        }

        return models;
    }
}