#include "stm32f4_discovery.h"
#include "AD9851.h"

#define AD9851_W_CLK_0_()	 GPIO_ResetBits(AD9851_PORT, AD9851_PIN_W_CLK)
#define AD9851_W_CLK_1_()	 GPIO_SetBits(AD9851_PORT, AD9851_PIN_W_CLK)


#define AD9851_FQ_UP_0_()	 GPIO_ResetBits(AD9851_PORT, AD9851_PIN_FQ_UP)
#define AD9851_FQ_UP_1_()	 GPIO_SetBits(AD9851_PORT, AD9851_PIN_FQ_UP)

 
#define AD9851_RESET_0_()	 GPIO_ResetBits(AD9851_PORT, AD9851_PIN_RESET)
#define AD9851_RESET_1_()	 GPIO_SetBits(AD9851_PORT, AD9851_PIN_RESET)


#define AD9851_BDATA_0_()	 GPIO_ResetBits(AD9851_PORT, AD9851_PIN_BDATA)
#define AD9851_BDATA_1_()	 GPIO_SetBits(AD9851_PORT, AD9851_PIN_BDATA)


void AD9851_Init(void)
{
	GPIO_InitTypeDef	 GPIO_InitStructure;
	
	RCC_AHB1PeriphClockCmd(AD9851_RCC,ENABLE);

	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;
	
	GPIO_InitStructure.GPIO_Pin =	AD9851_PIN_BDATA | AD9851_PIN_W_CLK | AD9851_PIN_FQ_UP |AD9851_PIN_RESET;
	GPIO_Init(AD9851_PORT, &GPIO_InitStructure);

	AD9851_W_CLK_0_();
	AD9851_FQ_UP_0_();


	AD9851_RESET_0_();
	AD9851_RESET_1_();
	AD9851_RESET_0_();

	AD9851_W_CLK_0_();
	AD9851_W_CLK_1_();
	AD9851_W_CLK_0_();

	AD9851_FQ_UP_0_();
	AD9851_FQ_UP_1_();
	AD9851_FQ_UP_0_();

}

void AD9851_WriteData(unsigned char w0,double frequency)
{
	unsigned char i=0,w=0;

	long y=0;

	double x=0.0f;
	//计算频率的HEX值
	x=4294967295.0/30.0;//适合180M晶振/180为最终时钟频率（或30M六倍频：ADS9851）
	
		//如果时钟频率不为180MHZ，修改该处的频率值，单位MHz	！！！
	frequency=frequency/1000000.0;
		
	frequency=frequency*x;
	
	y=(long)frequency;
	
	//写w4数frequency
	w	=(y>>=0);
		
	for(i=0;i<8;i++)
	{
		if((w>>i)&0x01)
		{
			AD9851_BDATA_1_();
		}
		else
		{
			AD9851_BDATA_0_();
		}
		AD9851_W_CLK_1_();
		AD9851_W_CLK_0_();
	}
	//写w3数据
	w=(y>>8);
	for(i=0;i<8;i++)
	{
		if((w>>i)&0x01)
		{
			AD9851_BDATA_1_();
		}
		else
		{
			AD9851_BDATA_0_();
		}
		AD9851_W_CLK_1_();
		AD9851_W_CLK_0_();
	}
	//写w2数据
	w=(y>>16);
	for(i=0;i<8;i++)
	{
		if((w>>i)&0x01)
		{
			AD9851_BDATA_1_();
		}
		else
		{
			AD9851_BDATA_0_();
		}
		AD9851_W_CLK_1_();
		AD9851_W_CLK_0_();
	}
	//写w1数据
	w=(y>>24);
	for(i=0;i<8;i++)
	{
		if((w>>i)&0x01)
		{
			AD9851_BDATA_1_();
		}
		else
		{
			AD9851_BDATA_0_();
		}
		AD9851_W_CLK_1_();
		AD9851_W_CLK_0_();
	}
	//写w0数据
	w=(unsigned char)w0;	 
	for(i=0;i<8;i++)
	{
		if((w>>i)&0x01)
		{
			AD9851_BDATA_1_();
		}
		else
		{
			AD9851_BDATA_0_();
		}
		AD9851_W_CLK_1_();
		AD9851_W_CLK_0_();
	}
	//移入始能
	AD9851_FQ_UP_1_();
	AD9851_FQ_UP_0_();
}



//---------------------------------------------------//
//串行写1000Hz程序
//AD9851_Init();
//AD9851_WriteData(0x01,1000);
//0X01为开六备频
//AD9851_WriteData(0x00,1000);
//0X00为不开六备频
//---------------------------------------------------//


