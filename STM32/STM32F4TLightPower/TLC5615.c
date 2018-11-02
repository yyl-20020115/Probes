#include "stm32f4_discovery.h"
#include "TLC5615.h"

#define TLC5615_DELAY() Delay_us(1)

void TLC5615_Init(void)
{
    GPIO_InitTypeDef   GPIO_InitStructure;
    
    RCC_AHB1PeriphClockCmd(TLC5615_GPIO_RCC_CS|TLC5615_GPIO_RCC_SCLK|TLC5615_GPIO_RCC_DIN,ENABLE);
 
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;
    GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;
    GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;
    
    GPIO_InitStructure.GPIO_Pin =  TLC5615_GPIO_PIN_CS;
    GPIO_Init(TLC5615_GPIO_PORT_CS, &GPIO_InitStructure);
    
    
    GPIO_InitStructure.GPIO_Pin =  TLC5615_GPIO_PIN_SCLK;
    GPIO_Init(TLC5615_GPIO_PORT_SCLK, &GPIO_InitStructure);
    
    
    GPIO_InitStructure.GPIO_Pin =  TLC5615_GPIO_PIN_DIN;
    GPIO_Init(TLC5615_GPIO_PORT_DIN, &GPIO_InitStructure);
    
   
}

//10 BIT DA
void TLC5615_WriteDAC(unsigned short DAValue)
{
	unsigned int i = 0;
    
	DAValue <<=6;
	
	GPIO_ResetBits(TLC5615_GPIO_PORT_CS, TLC5615_GPIO_PIN_CS);              //CS=0
    
	TLC5615_DELAY();
    
	GPIO_ResetBits(TLC5615_GPIO_PORT_SCLK, TLC5615_GPIO_PIN_SCLK);		    //SCLK=0	
	
	TLC5615_DELAY();
    //������12��ʱ��������,ÿ���������ص�
	//���ݱ�����,�γ�DA�������ǰ10��ʱ��
	//���������10λDA���ݣ�������ʱ������
	//Ϊ���λ(��ʱֵΪ0����
	
	for(i = 0;i<12;i++)
	{		
		if(DAValue & 0x8000)
		{
			GPIO_SetBits(TLC5615_GPIO_PORT_DIN, TLC5615_GPIO_PIN_DIN);      //DIN=1
		}
		else
		{
			GPIO_ResetBits(TLC5615_GPIO_PORT_DIN, TLC5615_GPIO_PIN_DIN);    //DIN=0		
		}
       	
			
		TLC5615_DELAY();
    	
		GPIO_SetBits(TLC5615_GPIO_PORT_SCLK, TLC5615_GPIO_PIN_SCLK);	    //SCLK=1
		
    TLC5615_DELAY();
    
		DAValue <<= 1;                   		                            //Shift 
		GPIO_ResetBits(TLC5615_GPIO_PORT_SCLK, TLC5615_GPIO_PIN_SCLK);      //SCLK=0		
    
		TLC5615_DELAY();
    
  }

	GPIO_SetBits(TLC5615_GPIO_PORT_CS, TLC5615_GPIO_PIN_CS);				//CS=1,	CS�������غ��½���ֻ����SCLKΪ�͵�ʱ��
	TLC5615_DELAY();
    
  GPIO_ResetBits(TLC5615_GPIO_PORT_SCLK, TLC5615_GPIO_PIN_SCLK);		        //SCLK=0
  TLC5615_DELAY();
    
}


