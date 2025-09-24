using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace умный_дом
{
    public class Room
    {
        public string Name { get; }
        public double Temperature { get; private set; }
        private bool isLightOn;

        public bool IsLightOn
        {
            get { return isLightOn; }
            set { isLightOn = value; }
        }

        public string LightState
        {
            get { return isLightOn ? "Включен" : "Выключен"; }
        }

        public event Action<string> OnLightStateChanged;
        public event Action<string> OnTemperatureChanged;

        public Room(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название комнаты не может быть пустым");

            Name = name;
        }

        public void TurnLightOn()
        {
            isLightOn = true;
            OnLightStateChanged?.Invoke($"Свет включен в комнате {Name}");
        }

        public void TurnLightOff()
        {
            isLightOn = false;
            OnLightStateChanged?.Invoke($"Свет выключен в комнате {Name}");
        }

        public void SetTemperature(double newTemperature)
        {
            if (newTemperature < 15 || newTemperature > 30)
                throw new ArgumentException("Температура должна быть в диапазоне от 15 до 30 градусов");

            Temperature = newTemperature;
            OnTemperatureChanged?.Invoke($"Температура изменена на {Temperature}°C в комнате {Name}");
        }

        public string GetRoomInfo()
        {
            return $"Комната: {Name}\nТемпература: {Temperature}°C\nСвет: {LightState}";
        }
    }
}