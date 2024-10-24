using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsUnitairesPourServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestsUnitairesPourServices.Data;
using TestsUnitairesPourServices.Models;
using TestsUnitairesPourServices.Exceptions;

namespace TestsUnitairesPourServices.Services.Tests
{
    /// <summary>
    /// Identifie la classe comme une classe de Test
    /// </summary>
    [TestClass()]
    public class CatsServiceTests
    {
        /// <summary>
        /// constructeur pour l'ensemble des tests de la classe
        /// </summary>
        DbContextOptions<ApplicationDBContext> options;

        //constantes
        private const int GRANDE_MAISON_ID = 1;
        private const int PETITE_MAISON_ID = 2;
        private const int BILLIE_ID = 1;
        private const int MUS_ID = 2;


        public CatsServiceTests()
        {
            //TODO : initialisation des options de la BD (ne pas oublier d'ajouter le package NuGet)
            options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "CatsService") //nom généré et choisi :))
                .UseLazyLoadingProxies(true) // Active le lazy loading
                .Options;
        }

        [TestInitialize]
        public void Init()
        {
            using ApplicationDBContext database = new ApplicationDBContext(options);

            //ajout de données de tests

            database.Cat.Add(new Cat()
            {
                Id = BILLIE_ID,
                Name = "Billie",
                Age = 4
            });

            House grandeMaison = new House()
            {
                Id = GRANDE_MAISON_ID,
                Address = "Grande maison qui peut accueillir plusieurs chats",
                OwnerName = "Zak"
            };

            House petiteMaison = new House()
            {
                Id = PETITE_MAISON_ID,
                Address = "Petite maison qui peut accueillir un chat ou pas du tout",
                OwnerName = "Bobby"
            };

            database.House.Add(grandeMaison);
            database.House.Add(petiteMaison);

            Cat mus = new Cat()
            {
                Id= MUS_ID,
                Name = "Mus",
                Age = 5,
                House = petiteMaison
            };

            database.Cat.Add(mus);

            database.SaveChanges();
        }

        [TestCleanup]
        public void Dispose()
        {
            using ApplicationDBContext database = new ApplicationDBContext(options);
            database.House.RemoveRange(database.House);
            database.Cat.RemoveRange(database.Cat);
            database.SaveChanges();
        }

        [TestMethod()]
        public void MoveTest()
        {
            using ApplicationDBContext database = new ApplicationDBContext(options);
            var catsService = new CatsService(database);
            var grandeMaison = database.House.Find(GRANDE_MAISON_ID);
            var petiteMaison = database.House.Find(PETITE_MAISON_ID);

            var déménagement = catsService.Move(MUS_ID, petiteMaison, grandeMaison);
            Assert.IsNotNull(déménagement);
        }

        [TestMethod()]
        public void MoveTestCatNotFound()
        {
            using ApplicationDBContext database = new ApplicationDBContext(options);
            var catsService = new CatsService(database);
            var grandeMaison = database.House.Find(GRANDE_MAISON_ID);
            var petiteMaison = database.House.Find(PETITE_MAISON_ID);

            //aucun chat avec l'id 101
            var déménagement = catsService.Move(101, petiteMaison, grandeMaison);
            Assert.IsNull(déménagement);
        }

        [TestMethod()]
        public void MoveTestNoHouse()
        {
            using ApplicationDBContext database = new ApplicationDBContext(options);
            var catsService = new CatsService(database);
            var grandeMaison = database.House.Find(GRANDE_MAISON_ID);
            var petiteMaison = database.House.Find(PETITE_MAISON_ID);

            //on vérifie que ça lance bien une exception
            Exception exception = Assert.ThrowsException<WildCatException>(() => catsService.Move(BILLIE_ID, grandeMaison, petiteMaison));

            //on vérifie que le message est le bon
            Assert.AreEqual("On n'apprivoise pas les chats sauvages", exception.Message);
        }

        [TestMethod()]
        public void MoveTestWrongHouse()
        {
            using ApplicationDBContext database = new ApplicationDBContext(options);
            var catsService = new CatsService(database);
            var grandeMaison = database.House.Find(GRANDE_MAISON_ID);
            var petiteMaison = database.House.Find(PETITE_MAISON_ID);

            //on vérifie que ça lance bien une exception
            Exception exception = Assert.ThrowsException<DontStealMyCatException>(() => catsService.Move(MUS_ID, grandeMaison, petiteMaison));

            //on vérifie que le message est le bon
            Assert.AreEqual("Touche pas à mon chat!", exception.Message);
        }
    }
}