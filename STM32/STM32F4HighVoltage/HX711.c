#include "HX711.h"
#include "stm32f4_discovery.h"

void Delay_ms(__IO uint32_t nTime);
void Delay_us(__IO uint32_t nTime);

//30g-range:
//0g
#define PRESET_ZEROLEVEL  8866940
//5g
#define PRESET_100GLEVEL  9037136
//10g
#define PRESET_500GLEVEL  9204689




int _ZeroLevel = PRESET_ZEROLEVEL;
int _100gLevel = PRESET_100GLEVEL;
int _500gLevel = PRESET_500GLEVEL;

int RealZeroLevel = 0;
int Real100gLevel = 0;
int Real500gLevel = 0;


#define		N	12
//PIN:PC6,PC7
#define HX_RCC RCC_AHB1Periph_GPIOC
#define HX_PORT GPIOC
#define ADSK_PIN GPIO_Pin_6
#define ADDO_PIN GPIO_Pin_7

int DataStore[N] ={0};

#define HX711_DELAY()   Delay_us(1)

void HX711_Init(void)
{
    GPIO_InitTypeDef   GPIO_InitStructure;
    
    RCC_AHB1PeriphClockCmd(HX711_RCC,ENABLE);

    GPIO_InitStructure.GPIO_Pin =  HX711_SCK_PIN;				 //PD0 = SCK, OUT MODE
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;
    GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_100MHz;
    GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;
    GPIO_Init(HX711_PORT, &GPIO_InitStructure);

    GPIO_InitStructure.GPIO_Pin =  HX711_DOUT_PIN;				 //PD1 = DOUT, IN-OUT MODE
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN;
    GPIO_Init(HX711_PORT, &GPIO_InitStructure);
}

int HX711_Read(void)
{
    int Value =0;
    int i = 0;

    //SCK =0;
    GPIO_ResetBits(HX711_PORT, HX711_SCK_PIN);
    
    while(GPIO_ReadInputDataBit(HX711_PORT,HX711_DOUT_PIN));
      
    for(i = 0;i<24;i++)
    {
        GPIO_SetBits(HX711_PORT,HX711_SCK_PIN); //SCK = 1
        Value <<=1;
				HX711_DELAY(); 
        GPIO_ResetBits(HX711_PORT,HX711_SCK_PIN); //SCK = 0
        if(GPIO_ReadInputDataBit(HX711_PORT,HX711_DOUT_PIN))
        {
            Value|=1;
        }
    }
    //sck 25 A 128
    GPIO_SetBits(HX711_PORT,HX711_SCK_PIN); //SCK = 1

		Value = Value ^ 0x800000;
 
    GPIO_ResetBits(HX711_PORT,HX711_SCK_PIN); //SCK = 0
    
    return Value;
}


//ÂË²¨
int HX711_GetMiddleFilterValue(int rc)
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
	sum = sum/i;

	return (int)sum;
}

void HX711_Calibrate(void)
{	
	int Delta = 0;
	int i = 0;
	for(i = 0;i<N;i++) // fill buffer
	{
		HX711_GetMiddleFilterValue(HX711_Read());
	}
	
	RealZeroLevel = HX711_GetMiddleFilterValue(HX711_Read());
	
	Delta = RealZeroLevel - _ZeroLevel;
  Real100gLevel =	_100gLevel + Delta;
	Real500gLevel = _500gLevel + Delta;
}

