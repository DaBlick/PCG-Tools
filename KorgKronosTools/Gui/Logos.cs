// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PcgTools.Gui
{
    public class Logos : List<Logo>
    {
        public Logos()
        {
            // In same order as docs google page.
            Add(new Logo("Steve Baker", string.Empty, string.Empty, 375));
            Add(new Logo("Adrian", "imprezariat.png", "http://www.imprezariat.pl", 500));
            Add(new Logo("Yuma", string.Empty, string.Empty, 1000));
            Add(new Logo("Steffen Traeger", string.Empty, string.Empty, 2000));
            Add(new Logo("Dreamland", "dreamland.jpg", "http://www.dreamland-recording.de", 1500)); // 2 CDs
            Add(new Logo("Toon Martens", "tmp.jpg", "http://www.toonmartensproject.net/", 1000)); // CD
            Add(new Logo("Fred Alberni/Farrokh Kouhang", string.Empty, string.Empty, 1000));
            Add(new Logo("phattbuzz", string.Empty, string.Empty, 1000));
            Add(new Logo("Wilton Vought", string.Empty, string.Empty, 1000));
            Add(new Logo("robbie50", string.Empty, string.Empty, 4500));
            Add(new Logo("Dave Gibson", string.Empty, string.Empty, 1500));
            Add(new Logo("Wan Kemper", string.Empty, string.Empty, 1500));
            Add(new Logo("Artur Dellarte", string.Empty, string.Empty, 1500));
            Add(new Logo("Michael Maschek", "celticvoyager.png", "https://www.facebook.com/celticvoyagerband", 1500));
            Add(new Logo("Jim Knopf", string.Empty, string.Empty, 2000));
            Add(new Logo("Olaf Arweiler", string.Empty, string.Empty, 2000));
            Add(new Logo("Martin Hines", string.Empty, string.Empty, 2000));
            Add(new Logo("Mathieu Maes", "cupsandplates.png", "http://partycoverband.wix.com/cupsandplates", 2000));
            Add(new Logo("Batisse", string.Empty, string.Empty, 2000));
            Add(new Logo("Traugott", string.Empty, string.Empty, 7000));
            Add(new Logo("Philip Joseph", string.Empty, string.Empty, 2500));
            Add(new Logo("Igor Elshaidt", string.Empty, string.Empty, 2500));
            Add(new Logo("Bruno Santos", string.Empty, string.Empty, 3000));
            Add(new Logo("Joe Keller", "keller12.jpg", "http://www.keller12.de", 3000));
            Add(new Logo("needamuse", string.Empty, string.Empty, 80000));
            Add(new Logo("Smyth Rocks", string.Empty, string.Empty, 10000));
            Add(new Logo("Sidney Leal", string.Empty, string.Empty, 500));
            Add(new Logo("Greg Heslington", string.Empty, string.Empty, 1500));
            Add(new Logo("Norman Clasper", string.Empty, string.Empty, 1200));
            Add(new Logo("Tim Godfrey", string.Empty, string.Empty, 1000));
            Add(new Logo("Jim G", string.Empty, string.Empty, 700));
            Add(new Logo("Jerry", string.Empty, string.Empty, 1000));
            Add(new Logo("Tim Möller", string.Empty, string.Empty, 500));
            Add(new Logo("Ralph Hopstaken", string.Empty, string.Empty, 1000));
            Add(new Logo("Kevin Nolan", string.Empty, string.Empty, 5000));
            Add(new Logo("Christian Moss", string.Empty, string.Empty, 500));
            Add(new Logo("Enrico Puglisi", "KronosPatchLab.png", "https://www.facebook.com/kronospatchlab", 1000));
            Add(new Logo("Mike Hildner", string.Empty, string.Empty, 5000));
            Add(new Logo("Miroslav Novak", string.Empty, string.Empty, 200));
            Add(new Logo("Daan Andriessen", "BK-facebook.gif", "www.studiodebovenkamer.nl", 2500));
            Add(new Logo("Synthesauris", "Synthesaurus.png", "https://www.patreon.com/synthesaurus", 3000));
        }


        /// <summary>
        /// Returns a random logo based on donated money.
        /// </summary>
        /// <returns></returns>
        public Logo GetRandomLogo()
        {
            var randomValue = new Random().Next(this.Sum(logo => logo.DonatedMoney));
            Thread.Sleep(10); // For the next random value (wait 10 ms, 100 ms is enough, 10 needs to be tested if needed)

            // Iterate through logos until found.
            var logoIndex = -1;
            var visitedValue = 0;

            while (true)
            {
                var logo = this[++logoIndex];
                if (visitedValue + logo.DonatedMoney > randomValue)
                {
                    break;
                }

                visitedValue += logo.DonatedMoney;
            }

            return this[logoIndex];
        }
    }
}
