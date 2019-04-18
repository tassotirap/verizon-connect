﻿namespace Verizon.Connect.Sender
{
    using System;
    using System.Threading.Tasks;
    using Verizon.Connect.Application.Interfaces;
    using Verizon.Connect.Domain.Plot.Enums;
    using Verizon.Connect.Domain.Plot.Models;

    public class SenderApplication
    {
        private readonly IPlotAppService plotAppService;
        private readonly Random random;
        private int vehicleId;
        private EventCode currentEventCode;
        private int lastTimeStamp;

        public SenderApplication(IPlotAppService plotAppService)
        {
            this.plotAppService = plotAppService;
            random = new Random();
        }

        public async Task Start(int vehicleId, int interval)
        {
            this.vehicleId = vehicleId;

            while (true)
            {
                PlotEntity plotEntity = GenerateRandoPlot();
                //await plotAppService.Register(plotEntity);
                Console.WriteLine($"Lat: {plotEntity.Lat}, Lon:{plotEntity.Lon}, EventCode:{plotEntity.EventCode}, V:{plotEntity.VId}, T:{plotEntity.TimeStamp}");

                await Task.Delay(interval);
            }
        }

        private PlotEntity GenerateRandoPlot()
        {
            // Start
            if (lastTimeStamp == 0)
            {
                currentEventCode = EventCode.IgnitionOn;
                return GeneratePlot(EventCode.IgnitionOn);
            }

            if (currentEventCode == EventCode.IgnitionOn)
            {
                currentEventCode = EventCode.Movement;
                return GeneratePlot(EventCode.Movement);
            }

            if (currentEventCode == EventCode.IgnitionOff)
            {
                currentEventCode = EventCode.IgnitionOn;
                return GeneratePlot(EventCode.IgnitionOn);
            }

            if (random.NextDouble() < 0.95)
            {
                currentEventCode = EventCode.Movement;
                return GeneratePlot(EventCode.Movement);
            }
            else
            {
                currentEventCode = EventCode.IgnitionOff;
                return GeneratePlot(EventCode.IgnitionOff);
            }
        }

        private PlotEntity GeneratePlot(EventCode eventCode)
        {
            PlotEntity plotEntity = new PlotEntity
            {
                EventCode = eventCode,
                Lat = $"la{lastTimeStamp}",
                Lon = $"lo{lastTimeStamp}",
                TimeStamp = $"t{lastTimeStamp}",
                VId = $"VId{lastTimeStamp}"
            };

            lastTimeStamp++;

            return plotEntity;
        }
    }
}