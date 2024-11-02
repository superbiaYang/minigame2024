using UnityEngine;
using Mirror;

public class CarModel
{
    public int Level { get; set; } = 1;
    public int AutoRecover { get; set; }
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public int Mass { get; set;}
    public float Drag { get; set; }
    public int TurnSpeed { get; set; }
    public int Acceleration { get; set; }
    public int Speed { get; set; }
    public int MaxSpeed { get; set; }
    public float DmgReduction { get; set; }
}

public static class CarModelSerialization
{
    public static void WriteCarModel(this NetworkWriter writer, CarModel car)
    {
        writer.WriteInt(car.Level);
        writer.WriteInt(car.AutoRecover);
        writer.WriteInt(car.Hp);
        writer.WriteInt(car.MaxHp);
        writer.WriteInt(car.Mass);
        writer.WriteFloat(car.Drag);
        writer.WriteInt(car.TurnSpeed);
        writer.WriteInt(car.Acceleration);
        writer.WriteInt(car.MaxSpeed);
        writer.WriteFloat(car.DmgReduction);
    }

    public static CarModel ReadCarModel(this NetworkReader reader)
    {
        CarModel car = new CarModel
        {
            Level = reader.ReadInt(),
            AutoRecover = reader.ReadInt(),
            Hp = reader.ReadInt(),
            MaxHp = reader.ReadInt(),
            Mass = reader.ReadInt(),
            Drag = reader.ReadFloat(),
            TurnSpeed = reader.ReadInt(),
            Acceleration = reader.ReadInt(),
            MaxSpeed = reader.ReadInt(),
            DmgReduction = reader.ReadFloat()
        };
        return car;
    }
}