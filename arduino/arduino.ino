#include <SimpleModbus\SimpleModbusSlave.h>

void setup()
{
	Serial.begin(9600);
  /* add setup code here */
	modbus_configure(9600, 1, 2, 10, 1);
}

unsigned int holdingRegs[10];
long temp = 0;
float Vref = 5000;
float Counts = 1024;
float coeff = Vref / Counts;
void loop()
{
	
	holdingRegs[0] = 200;
	

	for (uint i = 0; i < 6; i++)
	{
		temp = (analogRead(i)* coeff);
		holdingRegs[i+1] = (uint16_t)temp;
	}
		
	modbus_update(holdingRegs); 
}
