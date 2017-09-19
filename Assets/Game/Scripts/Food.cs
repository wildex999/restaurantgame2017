using Assets.Game.Scripts.Customers;
using ExitGames.Client.Photon;
using System.IO;
using UnityEngine;

namespace Assets.Game.Scripts
{
    public class Food
    {
        public string name;
        public CustomerGroup customer;

        public Food(string name, CustomerGroup customer)
        {
            this.name = name;
            this.customer = customer;
        }

        public static short Serialize(StreamBuffer outStream, object obj)
        {
            Food food = (Food)obj;
            long startLength = outStream.Length;
            BinaryWriter writer = new BinaryWriter(outStream);

            writer.Write(food.name);
            writer.Write(food.customer.photonView.viewID);

            return (short)(outStream.Length - startLength);
        }

        public static object Deserialize(StreamBuffer inStream, short length)
        {
            BinaryReader reader = new BinaryReader(inStream);

            string name = reader.ReadString();
            CustomerGroup customer = PhotonView.Find(reader.ReadInt32()).GetComponent<CustomerGroup>();

            return new Food(name, customer);
        }
    }
}
