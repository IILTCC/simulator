namespace simulator_main.dtos
{
    public class GetSimulationDto
    {
        public GetSimulationDto(string icdName)
        {
            this.IcdName = icdName;
        }
        public string IcdName { get; set; }
    }
}
