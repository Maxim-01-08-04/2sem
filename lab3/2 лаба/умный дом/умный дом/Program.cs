namespace умный_дом
{
    class Program
    {
        static void Main(string[] args)
        {
            Room kitchen = new Room("Кухня");

            kitchen.OnLightStateChanged += message => System.Console.WriteLine($"Событие: {message}");
            kitchen.OnTemperatureChanged += message => System.Console.WriteLine($"Событие: {message}");

            kitchen.TurnLightOn();
            kitchen.SetTemperature(22.5);

            System.Console.WriteLine(kitchen.GetRoomInfo());
            System.Console.WriteLine();

            kitchen.TurnLightOff();
            kitchen.SetTemperature(20.0);

            System.Console.WriteLine("После изменений:");
            System.Console.WriteLine(kitchen.GetRoomInfo());

            try
            {
                kitchen.SetTemperature(35);
            }
            catch (ArgumentException ex)
            {
                System.Console.WriteLine($"\nОшибка: {ex.Message}");
            }

            System.Console.WriteLine("\n--- Элементы управления ---");
            System.Console.WriteLine("Название комнаты: Кухня (TextBlock)");
            System.Console.WriteLine("Состояние света: Выключен (CheckBox)");
            System.Console.WriteLine("Температура: 20°C (Slider)");
        }
    }
}