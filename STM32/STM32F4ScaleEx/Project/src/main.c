#include "main.h"
//JTAG PINS: PB3,PB4,PA13,PA14,PA15
//TOUCH-SPI1:PA15 ->Conflict with JTAG

__IO u32 LocalTime=0; 

int RealZeroLevel = 0;
int Real100gLevel = 0;
int Real500gLevel = 0;

int _ZeroLevel = 0x00809444;
int __LowLevel = 0x0080E093;
int _HighLevel = 0x00821586;
int _GapWeight = 40;


//

#define		N	12
//PIN:PB6,PB7
#define HX_RCC RCC_AHB1Periph_GPIOB
#define HX_PORT GPIOB
#define ADSK_PIN GPIO_Pin_6
#define ADDO_PIN GPIO_Pin_7

int DataStore[N] ={0};

//HX711 AD读数
int ReadCount(void)
{
    int Count=0;
    int i=0;

    GPIO_ResetBits(HX_PORT,ADSK_PIN);
	
    Count = 0;
    while( GPIO_ReadInputDataBit(HX_PORT,ADDO_PIN));
    for (i = 0; i < 24; i++)
    {
				GPIO_SetBits(HX_PORT,ADSK_PIN);
        Count = Count << 1;
				GPIO_ResetBits(HX_PORT,ADSK_PIN);
        if(GPIO_ReadInputDataBit(HX_PORT,ADDO_PIN)) 
				{
					Count |=1;
				}
    }
		GPIO_SetBits(HX_PORT,ADSK_PIN);
    Count = Count ^ 0x800000;

		GPIO_ResetBits(HX_PORT,ADSK_PIN);

    return Count;
}


//滤波
int GetMiddleFilterValue(int rc)
{
	int max = 0;
	int min = 0;
	int64_t sum = 0;
	int i = 0;

	max = min = sum = DataStore[0] = rc; 	

	for(i=N-1; i!= 0; i--)
	{
		if(DataStore[i] > max) 
			max = DataStore[i];
		else if(DataStore[i] < min) 
			min = DataStore[i];

		sum = sum + DataStore[i];
		DataStore[i] = DataStore[i - 1];
	}
	
	i = N - 2;
	sum = sum - max - min;// + i/2;
	sum = (sum/i);

	return (int)sum;
}
void Scale_Init()
{
	GPIO_InitTypeDef GPIO_InitStructure;//初始化结构体

	RCC_AHB1PeriphClockCmd(HX_RCC, ENABLE);//GPIO外设时钟使能
	//PB7=ADDO
	GPIO_InitStructure.GPIO_Pin = ADDO_PIN;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN;//状态为输入
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;//无上下拉
	GPIO_Init(HX_PORT, &GPIO_InitStructure);//初始化
	//PB6=ADSK
	GPIO_InitStructure.GPIO_Pin =  ADSK_PIN;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;//状态为输入
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;//无上下拉
	GPIO_Init(HX_PORT, &GPIO_InitStructure);//初始化
}

void Calibrate(int32_t rc)
{	
	int Delta = 0;
	int i = 0;
	for(i = 0;i<N;i++) //fill buffer
	{
		GetMiddleFilterValue(ReadCount());
	}
	
	RealZeroLevel = GetMiddleFilterValue(ReadCount());
	
	Delta = RealZeroLevel - _ZeroLevel;
  Real100gLevel =	__LowLevel + Delta;
	Real500gLevel = _HighLevel + Delta;
		
}
int main(void)
{
	int value = 0;
	int middle = 0;
	
	Scale_Init();
	
	Calibrate(ReadCount());
	
	COM2Init(115200);

	while(1)
	{
		value = ReadCount();
	  middle = GetMiddleFilterValue(value);
	  //NOTICE: Weight gap is now less or equal to 65.535kg to be compatible with old version of hardwares.
		printf("W:%08X,%08X,%08X,%08X,%08X,%04X\n",value,middle,Real500gLevel,Real100gLevel,RealZeroLevel,_GapWeight);//12+40+9-5-4=52
		
	}
}


