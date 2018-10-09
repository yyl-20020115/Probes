#include "stm32f10x.h"
#include "delay.h"
#include "FSR.h"
#include "usart.h"
#include "adc.h"

u32 state = 0;
u32 val = 0;

int main(void)
{		
	delay_init();	
	NVIC_Configuration(); 	 //����NVIC�жϷ���2:2λ��ռ���ȼ���2λ��Ӧ���ȼ�
	uart_init(115200);	 //���ڳ�ʼ��Ϊ115200
	FSR_IO_Init();
	Adc_Init();

	delay_ms(200);

	while(1)
	{
		state = FSR_GPIO;
		
		val = Get_Adc(1);
		//18
		printf("PRESSURE:%08X\n",(val | state<<16));	
			
		delay_ms(20);
	}

}

