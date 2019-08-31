// (c) Copyright 2011-2019 MiKeSoft, Michel Keijzers, All rights reserved

using Common.Utils;

namespace PcgTools.Gui
{
    public class Logo
    {
        /// <summary>
        /// False = anonymous person.
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public string ImageName { get; private set; }


        /// <summary>
        /// IMPR: Check if really used in xaml.
        /// </summary>
        [UsedImplicitly]
// ReSharper disable MemberCanBePrivate.Global
        public string Url { get; private set; }
// ReSharper restore MemberCanBePrivate.Global


        /// <summary>
        /// Donated money in cents. One CD is considered as 10 euro, 2 CDs as 15 euros.
        /// </summary>
        public int DonatedMoney { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="imageName"></param>
        /// <param name="url"></param>
        /// <param name="donatedMoney"></param>
        public Logo(string name, string imageName, string url, int donatedMoney)
        {
            Name = name;
            ImageName = imageName;
            Url = url;
            DonatedMoney = donatedMoney;
        }
    }
}
