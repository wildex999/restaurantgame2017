using Assets.Game.Scripts.Customers;
using ExitGames.Client.Photon;
using System.IO;
using UnityEngine;

namespace Assets.Game.Scripts
{
    public class Order
    {
        public string name;
        public CustomerGroup customer;

        public Order(string name, CustomerGroup customer)
        {
            this.name = name;
            this.customer = customer;
        }

        public static short Serialize(StreamBuffer outStream, object obj)
        {
            Order order = (Order)obj;
            long startLength = outStream.Length;
            BinaryWriter writer = new BinaryWriter(outStream);

            writer.Write(order.name);
            writer.Write(order.customer.photonView.viewID);

            return (short)(outStream.Length - startLength);
        }

        public static object Deserialize(StreamBuffer inStream, short length)
        {
            BinaryReader reader = new BinaryReader(inStream);

            string name = reader.ReadString();
            CustomerGroup customer = PhotonView.Find(reader.ReadInt32()).GetComponent<CustomerGroup>();

            return new Order(name, customer);
        }
    }
}
