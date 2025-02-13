﻿using simulator_libary.icds;

namespace simulator_main.dtos
{
    public class GetSimulationDto
    {
        public IcdTypes IcdType { get; set; }

        public GetSimulationDto(IcdTypes icdType)
        {
            this.IcdType = icdType;
        }
    }
}
