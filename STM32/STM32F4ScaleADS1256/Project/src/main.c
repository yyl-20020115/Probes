#include "main.h"

__IO u32 LocalTime=0; 

int main(void)
{
  uint32_t value = 0;

	ADS1256_Init();
	
	COM1Init(115200);

	while(1)
	{
		value = (uint32_t) ADS1256_ReadADC_Signed(0);
		printf("A:%08X\n",value);
		delay_us(10*1000);//10ms
	}
}

