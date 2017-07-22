#include <SimpleModbus\SimpleModbusSlave.h>

void setup()
{
	//Setting up modbus
	modbus_configure(9600, 1, 2, 10, 1);
}

//0 - reg for check
// 1-6 regs for Analog Inputs
unsigned int holdingRegs[7];
long temp = 0;
float Vref = 5000;
float Counts = 1024;
float coeff = Vref / Counts;

void loop()
{
	holdingRegs[0] = 200;
	
	//Update registers
	for (uint8_t i = 0; i < 6; i++)
	{
		temp = (analogRead(i)* coeff);
		holdingRegs[i+1] = (uint16_t)temp;
	}

	modbus_update(holdingRegs); 
}
