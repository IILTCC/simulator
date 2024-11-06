namespace simulator_main.dtos
{
    public class GetSimulationDto
    {
        public string IcdName { get; set; }

        public GetSimulationDto(string icdName)
        {
            this.IcdName = icdName;
        }
    }
}
