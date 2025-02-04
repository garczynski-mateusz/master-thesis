using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CateringBackend.Domain.Entities;

namespace CateringBackend.Domain.Utilities
{
    public class MealsAndDietsSeedDataProvider
    {
        public List<Diet> DietsToSeed { get; private set; } = new List<Diet>(); 
        public List<Meal> MealsToSeed { get; private set; } = new List<Meal>();

        public MealsAndDietsSeedDataProvider()
        {
            AddDietaVegan();
            AddDietaByMagdaGesler();
            AddDietaStandard();
            AddDietaSport();
        }

        private void AddDietaVegan()
        {
            var m1 = Meal.Create(
                name: "Słonecznikowa pasta z bakłażana z żytnim pieczywem na zakwasie i rzodkiewką",
                ingredients: "słonecznik, bakłażan, żytnie pieczywo, rzodkiewka, ryż",
                allergens: "zboża z glutenem",
                384,
                true);

            var m2 = Meal.Create(
                name: "Ptasie mleczko chia z galaretką brzoskwiniową i płatkami migdałowym",
                ingredients: "ptasie mleczko, chia, galaretka brzoskwiniowa, płatki migdałowe",
                allergens: "orzechy",
                238,
                true);

            var m3 = Meal.Create(
                name: "Curry z czerwoną fasolą, batatami i warzywami z brązowym ryżem",
                ingredients: @"Curry z batatami ziemniakami i warzywami, Mleczko kokosowe, Woda, Ziemniaki, Bataty, Cebula biała, Groszek cukrowy, Szpinak, Olej rzepakowy, Imbir, Czosnek, Papryczka chili, Fasola czerwona, Kolendra, Ryż brązowy",
                allergens: "mleko i produkty pochodne",
                847,
                true);

            var m4 = Meal.Create(
                name: "Kokosowe ciasteczka jaglane",
                ingredients: @"Kasza jaglana, Daktyle, suszone, Wiórki kokosowe, Migdały, Olej kokosowy",
                allergens: "Orzechy, Nasiona sezamu i produkty pochodne",
                384,
                true);

            var m5 = Meal.Create(
                name: "Ratatouille z zieloną soczewicą",
                ingredients: @"Kasza jaglana, Daktyle, suszone, Wiórki kokosowe, Migdały, Olej kokosowy",
                allergens: "Pietruszka, natka, Pomidory całe bez skórki, Papryka czerwona, Bakłażan, Cukinia, Cebula biała, Oliwa z oliwek, Czosnek, Soczewica zielona",
                690,
                true);

            var dietMeals = new List<Meal> {m1, m2, m3, m4, m5};

            var description = "Dieta wegańska to taka, która nie zawiera produktów pochodzenia zwierzęcego, a więc mięsa (w tym ryb i owoców morza), nabiału, jaj itp. Chociaż początkowo może wydawać się trudną i niedoborową, Światowa Organizacja Zdrowia jasno stwierdza, że odpowiednio zbilansowana jest zdrowa i bezpieczna dla osób na każdym etapie życia, również dla kobiet w ciąży, małych dzieci, młodzieży i sportowców, a jej rosnąca popularność sprawia, że współcześnie kupienie wegańskich produktów nie stanowi już żadnego wyzwania.";

            var diet1 = Diet.Create(
                title: "Dieta Vegan-  Basic",
                description: description,
                price: 59,
                meals: new List<Meal> { m1, m3, m5 });

            var diet2 = Diet.Create(
                title: "Dieta Vegan - Normal z podwieczorkiem",
                description: description,
                price: 59,
                meals: new List<Meal> { m1, m3, m4, m5 });

            var diet3 = Diet.Create(
                title: "Dieta Vegan - Normal z drugim śniadaniem",
                description: description,
                price: 59,
                meals: new List<Meal> { m1, m2, m3, m5 });

            var diet4 = Diet.Create(
                title: "Dieta Vegan - Premium",
                description: description,
                price: 59,
                meals: new List<Meal> { m1, m2, m3, m4, m5 });

            var diet5 = Diet.Create(
                title: "Dieta Vegan - śniadania",
                description: description,
                price: 59,
                meals: new List<Meal> { m1 });

            MealsToSeed.AddRange(dietMeals);
            DietsToSeed.Add(diet1);
            DietsToSeed.Add(diet2);
            DietsToSeed.Add(diet3);
            DietsToSeed.Add(diet4);
            DietsToSeed.Add(diet5);
        }

        private void AddDietaByMagdaGesler()
        {
            var m1 = Meal.Create(
                name: "Tost francuski z chałki z domowymi konfiturami i serkiem",
                ingredients: @"Chałka drożdżowa, Jaja kurze, Mleko spożywcze, Maliny, Skrobia ziemniaczana, Konfitura z pomarańczy, Mięta, mietana 22% tłuszczu, Miód pszczeli",
                allergens: "Zboża zawierające gluten, Jaja i produkty pochodne, Mleko i produkty pochodne (łącznie z laktozą)",
                420,
                false);

            var m2 = Meal.Create(
                name: "Racuchy z jabłkiem i bananem z serkiem oraz musem szampańskim",
                ingredients: @"Kefir, Mąka pszenna, Jabłko, Banan, Jaja kurze, Miód pszczeli, Olej rzepakowy, Proszek do pieczenia, Soda oczyszczona",
                allergens: "Zboża zawierające gluten, Jaja i produkty pochodne, Mleko i produkty pochodne (łącznie z laktozą), Dwutlenek siarki i siarczyny",
                569,
                true);

            var m3 = Meal.Create(
                name: "Halibut w sosie grzybowo - pieprzowym z karmelizowanymi warzywami",
                ingredients: @"Halibut, Seler, Cebula biała, Sok z cytryny, Masło 82%, Oliwa z oliwek, Tymianek, Śmietanka 18%, Grzyby shimeji, Koniak, Szalotka, Pieprz fermentowany, Wino białe wytrawne, Czosnek, Oliwa z oliwek, Tymianek, Ziemniaki fioletowe",
                allergens: "Ryby i produkty pochodne, Mleko i produkty pochodne (łącznie z laktozą), Orzechy",
                835,
                false);


            var m4 = Meal.Create(
                name: "Mus czekoladowy z sosem malinowym",
                ingredients: @"Czekolada biała, Maliny, Cukier, Śmietana, Jaja kurze",
                allergens: "Jaja i produkty pochodne, Mleko i produkty pochodne (łącznie z laktozą)",
                478,
                false);


            var m5 = Meal.Create(
                name: "Pieczone ziemniaczki w stylu włoskim ",
                ingredients: @"Bazylia świeża, Ziemniaki, Pomidor koktajlowy, Cebula biała, Ser, parmezan, Olej rzepakowy, Oliwa z oliwek, Czosnek, Tymianek, ",
                allergens: "Mleko i produkty pochodne (łącznie z laktozą)",
                278,
                false);

            var dietMeals = new List<Meal> { m1, m2, m3, m4, m5 };

            var description =
                "Magda Gessler, właśc. Magdalena Daria Gessler, z domu Ikonowicz (ur. 10 lipca 1953 w Komorowie) – polska restauratorka, właścicielka lub współwłaścicielka kilkunastu restauracji, malarka, felietonistka i osobowość telewizyjna. 20 maja 1982 Gessler wzięła ślub cywilny z Volkhartem Müllerem (1942–1987), korespondentem tygodnika „Der Spiegel” w Madrycie. 13 lipca 1985 wzięli ślub kościelny. Mają syna, Tadeusza.";

            var diet1 =  Diet.Create(
                title: "Dieta by Magda Gesler - Basic",
                description: description,
                price: 45,
                meals: new List<Meal> { m1, m3, m5 });

            var diet2 = Diet.Create(
                title: "Dieta by Magda Gesler - Normal z podwieczorkiem",
                description: description,
                price: 45,
                meals: new List<Meal> { m1, m3, m4, m5 });

            var diet3 = Diet.Create(
                title: "Dieta by Magda Gesler - Normal z drugim śniadaniem",
                description: description,
                price: 45,
                meals: new List<Meal> { m1, m2, m3, m5 });

            var diet4 = Diet.Create(
                title: "Dieta by Magda Gesler - Premium",
                description: description,
                price: 45,
                meals: new List<Meal> { m1, m2, m3, m4, m5 });

            MealsToSeed.AddRange(dietMeals);
            DietsToSeed.Add(diet1);
            DietsToSeed.Add(diet2);
            DietsToSeed.Add(diet3);
            DietsToSeed.Add(diet4);
        }

        private void AddDietaStandard()
        {
            var m1 = Meal.Create(
                name: "Mango sticky rice",
                ingredients: "Mango, Miód pszczeli, Mleczko kokosowe, Przyprawy, Ryż basmati",
                allergens: "",
                420,
                false);

            var m2 = Meal.Create(
                name: "Placuszek ziemniaczany z dipem z sera pleśniowego z marchewką i kalarepką",
                ingredients: @"Jogurt grecki, Śmietana kwaśna, Ser lazur, Ocet winny, Miód pszczeli, Szczypiorek, Kalarepa, Marchew, Ziemniaki, Jaja kurze, Ser, cheddar, Czosnek",
                allergens: "Jaja i produkty pochodne, Mleko i produkty pochodne (łącznie z laktozą)",
                682,
                false);

            var m3 = Meal.Create(
                name: "Spaghetti tuńczykowe",
                ingredients: @"Makaron pełnoziarnisty, Oliwa z oliwek, Pietruszka, Pomidor koktajlowy, Śmietanka 18%, Tuńczyk w sosie własnym, Papryka zielona, Cebula czerwona, Olej rzepakowy, Przyprawy, Skrobia ziemniaczana, Papryczka jalapeno",
                allergens: "Zboża zawierające gluten, Ryby i produkty pochodne, Mleko i produkty pochodne (łącznie z laktozą)",
                962,
                false);

            var m4 = Meal.Create(
                name: "Sernik z musem orzechowo- kawowym",
                ingredients: @"Twaróg mielony, Śmietanka 18%, Miód pszczeli, Ciasteczka owsiane, Jaja kurze, Masło 82% tłuszczu, Orzechy włoskie, Skrobia ziemniaczana, Woda, Syrop klonowy, Kawa rozpuszczalna",
                allergens: "Zboża zawierające gluten, Jaja i produkty pochodne",
                382,
                false);

            var m5 = Meal.Create(
                name: "Azjatycka zupa cytrynowa z makaronem sojowym",
                ingredients: @"Woda, Mleczko kokosowe, Kapusta kiszona, Papryka czerwona, Cebula czerwona, Sok z cytryny, Sos sojowy jasny, Imbir, świeży, Pasta Tom Kha, Miód pszczeli, Czosnek, Olej rzepakowy, Papryczka chili, Przyprawy, Trawa cytrynowa, Sos rybny, Makaron sojowy",
                allergens: "Soja i produkty pochodne",
                538,
                false);

            var description = @"Box dedykowany dla osób, które chcą oszczędzić czas na gotowaniu posiłków do pracy.

Dbamy o to żeby dania były zawsze różnorodne i idealnie zbilansowane, bogate w składniki odżywcze, abyś w pracy otrzymał maximum energii.

Składa się z: śniadania, obiadu i podwieczorka.

Dzięki dwóm opcjom kalorycznym do wyboru, resztę potrzebnych kalorii łatwo uzupełnisz w domu.";

            var diet1 = Diet.Create(
                title: "Dieta Standard Basic",
                description: description,
                price: 39,
                meals: new List<Meal> { m1, m3, m5 });

            var diet2 = Diet.Create(
                title: "Dieta Standard Normal",
                description: description,
                price: 49,
                meals: new List<Meal> { m1, m2, m3, m5 });

            var diet3 = Diet.Create(
                title: "Dieta Standard Premium",
                description: description,
                price: 59,
                meals: new List<Meal> { m1, m2, m3, m4, m5 });

            MealsToSeed.AddRange(new List<Meal> { m1, m2, m3, m4, m5 });
            DietsToSeed.Add(diet1);
            DietsToSeed.Add(diet2);
            DietsToSeed.Add(diet3);
        }

        void AddDietaSport()
        {
            var m1 = Meal.Create(
                name: "Kanapka ze słonecznikową pastą z bakłażana i warzywami, Koktajl proteinowy owocowy",
                ingredients: "Chleb żytni razowy, Mleko spożywcze, Gruszka, jabłko, czerwona porzeczka, agrest, Białko serwatkowe, Miód pszczeli, Bakłażan, Słonecznik, nasiona, Cebula czerwona, Suszone pomidory w oleju, Olej rzepakowy, Rzodkiewka, Szczypiorek",
                allergens: "Zboża zawierające gluten, Mleko i produkty pochodne (łącznie z laktozą)",
                620,
                false);

            var m2 = Meal.Create(
                name: "Domowy deser monte",
                ingredients: @"Serek naturalny, Miód pszczeli, Kakao naturalne, Ekstrakt waniliowy, Orzechy laskowe",
                allergens: "Orzechy, Mleko i produkty pochodne (łącznie z laktozą)",
                340,
                false);

            var m3 = Meal.Create(
                name: "Curry z indykiem, batatami i warzywami z brązowym ryżem, Krem z zielonych warzyw",
                ingredients: @"Mleczko kokosowe, Woda, Ziemniaki, Bataty, Cebula biała, Groszek cukrowy, Szpinak, Olej rzepakowy, Imbir, Czosnek, Papryczka chili, Kolendra, Woda, Brokuły, Groszek zielony, mrożony, Cebula biała, Masło 82% tłuszczu, Czosnek, Przyprawy, Mięso z piersi indyka, Olej rzepakowy, Ryż brązowy",
                allergens: "Mleko i produkty pochodne (łącznie z laktozą)",
                824,
                false);

            var m4 = Meal.Create(
                name: "Bananowiec z powidłami i tahini, Smoothie jagodowe",
                ingredients: @"Banan, Powidła śliwkowe, Miód pszczeli, Mąka pszenna, Tahini, Jaja kurze, Mąka pszenna, Ocet winny, Ekstrakt waniliowy, Soda oczyszczona, Proszek do pieczenia, Przyprawy, Cynamon, Sok jabłkowy, tłoczony, Jagody",
                allergens: "Zboża zawierające gluten, ",
                254,
                true);

            var m5 = Meal.Create(
                name: "Zupa marokańska z kurczakiem",
                ingredients: @"Cytryna, Daktyle, suszone, Mięso z piersi kurczaka, Olej rzepakowy, Sezam, Woda, Pomidory całe bez skórki, Cieciorka, Marchew, Soczewica czerwona, Soczewica zielona, Cynamon",
                allergens: "Nasiona sezamu i produkty pochodne, Dwutlenek siarki i siarczyny",
                523,
                false);

            var dietMeals = new List<Meal> { m1, m2, m3, m4, m5 };

            var description =
                "Dieta Sport doskonale nadaje się zarówno dla osób, które mają zwiększone zapotrzebowanie energetyczne jak i tych aktywnych fizycznie, chcących zmienić swoje nawyki żywieniowe i dzięki temu osiągać lepsze rezultaty.";

            var diet1 = Diet.Create(
                title: "Dieta Sport - Basic",
                description: description,
                price: 42,
                meals: new List<Meal> { m1, m3, m5 });

            var diet2 = Diet.Create(
                title: "Dieta Sport - Normal z drugim śniadaniem",
                description: description,
                price: 49,
                meals: new List<Meal> { m1, m2, m3, m5 });

            var diet3 = Diet.Create(
                title: "Dieta Sport - Normal z podwieczorkiem",
                description: description,
                price: 49,
                meals: new List<Meal> { m1, m3, m4, m5 });

            var diet4 = Diet.Create(
                title: "Dieta Sport - Normal z Premium",
                description: description,
                price: 59,
                meals: new List<Meal> { m1, m2, m3, m4, m5 });


            MealsToSeed.AddRange(dietMeals);
            DietsToSeed.Add(diet1);
            DietsToSeed.Add(diet2);
            DietsToSeed.Add(diet3);
            DietsToSeed.Add(diet4);
        }
    }
}
