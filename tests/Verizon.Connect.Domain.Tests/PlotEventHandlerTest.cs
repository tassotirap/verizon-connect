namespace Verizon.Connect.Domain.Tests
{
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Verizon.Connect.Domain.Plot.Enums;
    using Verizon.Connect.Domain.Plot.EventHandlers;
    using Verizon.Connect.Domain.Plot.Events;
    using Verizon.Connect.Domain.Plot.Models;
    using Verizon.Connect.Tests.Logger;
    using Verizon.Connect.Tests.Repository;

    using Xunit;
    using Xunit.Abstractions;

    public class PlotEventHandlerTest
    {
        private const int Vehicleid = 10;

        private readonly PlotEventHandler plotEventHandler;

        private readonly PlotInMemoryRepository plotEventRepository;

        public PlotEventHandlerTest(ITestOutputHelper output)
        {
            this.plotEventRepository = new PlotInMemoryRepository();

            var logger = LoggerFactory.CreateLogger<PlotEventHandler>(output);

            this.plotEventHandler = new PlotEventHandler(this.plotEventRepository, logger);
        }

        [Fact(DisplayName = "[Exception] - Register IgnitionOff event")]
        public async Task RegisterIgnitionOffEventException()
        {
            this.plotEventRepository.Items = null;

            var result = await this.RegisterIgnitionOffEvent(0);

            Assert.False(result);
        }

        [Fact(DisplayName = "[Sucess] - Register IgnitionOff event")]
        public async Task RegisterIgnitionOffEventSuccess()
        {
            var result = await this.RegisterIgnitionOffEvent(0);

            Assert.True(result);
            Assert.Equal(1, this.plotEventRepository.Items.Count);
            Assert.True(this.plotEventRepository.Items.ContainsKey("Plot:VId10:t0"));
        }

        [Fact(DisplayName = "[Sucess] - Register IgnitionOn, Moviment and IgnitionOff event")]
        public async Task RegisterIgnitionOnAndMovementAndIgnitionOffEventSuccess()
        {
            var result = await this.RegisterIgnitionOnEvent(0);

            result &= await this.RegisterMovementEvent(1);

            result &= await this.RegisterIgnitionOffEvent(2);

            Assert.True(result);
            Assert.Equal(4, this.plotEventRepository.Items.Count);
            Assert.True(this.plotEventRepository.Items.ContainsKey("Plot:VId10:t0"));
            Assert.True(this.plotEventRepository.Items.ContainsKey("Plot:VId10:LastIgnitionOn"));
            Assert.True(this.plotEventRepository.Items.ContainsKey("Plot:VId10:t1"));
            Assert.True(this.plotEventRepository.Items.ContainsKey("Plot:VId10:t2"));

            var movementEvent = (PlotEntity)this.plotEventRepository.Items["Plot:VId10:t1"];
            Assert.Equal("t0", movementEvent.JourneyStart);
            Assert.Null(movementEvent.JourneyEnd);

            var ignitionOffEvent = (PlotEntity)this.plotEventRepository.Items["Plot:VId10:t2"];
            Assert.Equal("t0", ignitionOffEvent.JourneyStart);
            Assert.Equal("t2", ignitionOffEvent.JourneyEnd);
        }

        [Fact(DisplayName = "[Sucess] - Register IgnitionOn and Moviment event")]
        public async Task RegisterIgnitionOnAndMovementEventSuccess()
        {
            var result = await this.RegisterIgnitionOnEvent(0);

            result &= await this.RegisterMovementEvent(1);

            Assert.True(result);
            Assert.Equal(3, this.plotEventRepository.Items.Count);
            Assert.True(this.plotEventRepository.Items.ContainsKey("Plot:VId10:t0"));
            Assert.True(this.plotEventRepository.Items.ContainsKey("Plot:VId10:LastIgnitionOn"));
            Assert.True(this.plotEventRepository.Items.ContainsKey("Plot:VId10:t1"));

            var movementEvent = (PlotEntity)this.plotEventRepository.Items["Plot:VId10:t1"];
            Assert.Equal("t0", movementEvent.JourneyStart);
            Assert.Null(movementEvent.JourneyEnd);
        }

        [Fact(DisplayName = "[Exception] - Register IgnitionOn event")]
        public async Task RegisterIgnitionOnEventException()
        {
            this.plotEventRepository.Items = null;

            var result = await this.RegisterIgnitionOnEvent(0);

            Assert.False(result);
        }

        [Fact(DisplayName = "[Sucess] - Register IgnitionOn event")]
        public async Task RegisterIgnitionOnEventSuccess()
        {
            var result = await this.RegisterIgnitionOnEvent(0);

            Assert.True(result);
            Assert.Equal(2, this.plotEventRepository.Items.Count);
            Assert.True(this.plotEventRepository.Items.ContainsKey("Plot:VId10:t0"));
            Assert.True(this.plotEventRepository.Items.ContainsKey("Plot:VId10:LastIgnitionOn"));
        }

        [Fact(DisplayName = "[Exception] - Register IgnitionOff event")]
        public async Task RegisterMovementEventException()
        {
            this.plotEventRepository.Items = null;

            var result = await this.RegisterMovementEvent(0);

            Assert.False(result);
        }

        [Fact(DisplayName = "[Sucess] - Register IgnitionOff event")]
        public async Task RegisterMovementEventSuccess()
        {
            var result = await this.RegisterMovementEvent(0);

            Assert.True(result);
            Assert.Equal(1, this.plotEventRepository.Items.Count);
            Assert.True(this.plotEventRepository.Items.ContainsKey("Plot:VId10:t0"));
        }

        private async Task<bool> RegisterIgnitionOffEvent(int timeStamp)
        {
            var registerPlotEvent = new RegisterPlotEvent(new PlotEntity(Vehicleid, timeStamp, EventCode.IgnitionOff));
            return await this.plotEventHandler.EventReceived(registerPlotEvent);
        }

        private async Task<bool> RegisterIgnitionOnEvent(int timeStamp)
        {
            var registerPlotEvent = new RegisterPlotEvent(new PlotEntity(Vehicleid, timeStamp, EventCode.IgnitionOn));
            return await this.plotEventHandler.EventReceived(registerPlotEvent);
        }

        private async Task<bool> RegisterMovementEvent(int timeStamp)
        {
            var registerPlotEvent = new RegisterPlotEvent(new PlotEntity(Vehicleid, timeStamp, EventCode.Movement));
            return await this.plotEventHandler.EventReceived(registerPlotEvent);
        }
    }
}