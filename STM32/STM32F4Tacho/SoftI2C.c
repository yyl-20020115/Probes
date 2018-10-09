#include "SoftI2C.h"

void SOFTI2C_SCL_0(SOFTI2C* s)		{  GPIO_ResetBits(s->SCL_Port,s->SCL_Pin);}
void SOFTI2C_SCL_1(SOFTI2C* s) 		{  GPIO_SetBits(s->SCL_Port,s->SCL_Pin);}
void SOFTI2C_SDA_0(SOFTI2C* s) 		{  GPIO_ResetBits(s->SDA_Port,s->SDA_Pin);}
void SOFTI2C_SDA_1(SOFTI2C* s) 		{  GPIO_SetBits(s->SDA_Port,s->SDA_Pin);}

unsigned char   SOFTI2C_SDA_STATE(SOFTI2C* s)        { return GPIO_ReadInputDataBit(s->SDA_Port,s->SDA_Pin) != 0; }

extern void Delay_us(unsigned int nTime);

/*******************************************************************************
* 函数名称:SOFTI2C_Delay                                                                     
* 描    述:延时函数                                                                     
*                                                                               
* 输    入:无                                                                     
* 输    出:无                                                                     
* 返    回:无                                                                     
* 作    者:                                                                     
* 修改日期:2010年6月8日                                                                    
*******************************************************************************/
void SOFTI2C_NOP(SOFTI2C* s)
{
    unsigned int i = 20, j = 0;
    unsigned int sum = 0;

    if(s->DelayValue>0)
    {
        Delay_us(s->DelayValue);
    }
    else
    {

        while(i--)
        {
            for (j = 0; j < 10; j++)
            {
                sum += i; 
            }
        }
        sum = i;
    }
}

/*******************************************************************************
* 函数名称:SOFTI2C_Initialize                                                                     
* 描    述:I2C初始化函数                                                                     
*                                                                               
* 输    入:无                                                                     
* 输    出:无                                                                     
* 返    回:无                                                                     
* 作    者:                                                                     
* 修改日期:2010年6月8日                                                                    
*******************************************************************************/
void SOFTI2C_Init(SOFTI2C* s)
{
    GPIO_InitTypeDef GPIO_InitStructure;
    GPIO_InitStructure.GPIO_Speed=GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode=GPIO_Mode_OUT;
    GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;

    GPIO_InitStructure.GPIO_Pin = s->SCL_Pin; 
    GPIO_Init(s->SCL_Port, &GPIO_InitStructure);

    //SDA: OD Mode, BOTH IN AND OUT
    GPIO_InitStructure.GPIO_Mode=GPIO_Mode_OUT;
    GPIO_InitStructure.GPIO_OType = GPIO_OType_OD;
    GPIO_InitStructure.GPIO_Pin = s->SDA_Pin; 
    GPIO_Init(s->SDA_Port, &GPIO_InitStructure);


    SOFTI2C_SDA_1(s);
    SOFTI2C_SCL_0(s); 
}

/*******************************************************************************
* 函数名称:SOFTI2C_START                                                                     
* 描    述:发送启动                                                                     
*                                                                               
* 输    入:无                                                                     
* 输    出:无                                                                     
* 返    回:无                                                                     
* 作    者:                                                                     
* 修改日期:2010年6月8日                                                                    
*******************************************************************************/
unsigned char SOFTI2C_START(SOFTI2C* s)
{ 
    SOFTI2C_SDA_1(s); 
    SOFTI2C_NOP(s);

    SOFTI2C_SCL_1(s); 
    SOFTI2C_NOP(s);    

    if(!SOFTI2C_SDA_STATE(s))
    {
        return s->State = SOFTI2C_BUS_BUSY;
    }
    
    SOFTI2C_SDA_0(s);
    SOFTI2C_NOP(s);

    SOFTI2C_SCL_0(s);  
    SOFTI2C_NOP(s); 

    if(SOFTI2C_SDA_STATE(s))
    {
        return s->State = SOFTI2C_BUS_ERROR;
    } 

    return SOFTI2C_READY;
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  SOFTI2C_START_SHT 
* 
* @Returns:   
*/
/* --------------------------------------------------------------------------*/
unsigned char SOFTI2C_START_SHT(SOFTI2C* s)
{
    SOFTI2C_SDA_1(s);
    SOFTI2C_SCL_0(s);
    SOFTI2C_NOP(s);

    SOFTI2C_SDA_1(s); 
    SOFTI2C_SCL_1(s); 
    SOFTI2C_NOP(s);

    if(!SOFTI2C_SDA_STATE(s))
    {
        return s->State = SOFTI2C_BUS_BUSY;
    }
    
    SOFTI2C_SDA_0(s);
    SOFTI2C_NOP(s);

    SOFTI2C_SCL_0(s);  
    SOFTI2C_NOP(s); 

    SOFTI2C_SCL_1(s);
    SOFTI2C_NOP(s);

    SOFTI2C_SDA_1(s);
    SOFTI2C_NOP(s);

    SOFTI2C_SCL_0(s);
    SOFTI2C_NOP(s);

    return s->State = SOFTI2C_READY;
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  SOFTI2C_STOP 
*/
/* --------------------------------------------------------------------------*/
void SOFTI2C_STOP(SOFTI2C* s)
{
    SOFTI2C_SDA_0(s); 
    SOFTI2C_NOP(s);

    SOFTI2C_SCL_1(s); 
    SOFTI2C_NOP(s);    

    SOFTI2C_SDA_1(s);
    SOFTI2C_NOP(s);
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  SOFTI2C_SendACK 
*/
/* --------------------------------------------------------------------------*/
void SOFTI2C_SendACK(SOFTI2C* s)
{
    SOFTI2C_SDA_0(s);
    SOFTI2C_NOP(s);
    SOFTI2C_SCL_1(s);
    SOFTI2C_NOP(s);
    SOFTI2C_SCL_0(s); 
    SOFTI2C_NOP(s); 
    SOFTI2C_SDA_1(s);
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  SOFTI2C_SendNACK 
*/
/* --------------------------------------------------------------------------*/
void SOFTI2C_SendNACK(SOFTI2C* s)
{
    SOFTI2C_SDA_1(s);
    SOFTI2C_NOP(s);
    SOFTI2C_SCL_1(s);
    SOFTI2C_NOP(s);
    SOFTI2C_SCL_0(s); 
    SOFTI2C_NOP(s);
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  SOFTI2C_SendByte 
* 
* @Param: Data
* 
* @Returns:   
*/
/* --------------------------------------------------------------------------*/
unsigned char SOFTI2C_SendByte(SOFTI2C* s,unsigned char Data)
{
    unsigned char i = 0;
    
    SOFTI2C_SCL_0(s);
    for(i=0;i<8;i++)
    {  
        //---------数据建立----------
        if(Data&0x80)
        {
            SOFTI2C_SDA_1(s);
        }
        else
        {
            SOFTI2C_SDA_0(s);
        } 
        Data<<=1;
        SOFTI2C_NOP(s);
        //---数据建立保持一定延时----

        //----产生一个上升沿[正脉冲] 
        SOFTI2C_SCL_1(s);
        SOFTI2C_NOP(s);
        SOFTI2C_SCL_0(s);
        SOFTI2C_NOP(s);//延时,防止SCL还没变成低时改变SDA,从而产生START/STOP信号
        //---------------------------   
    }
    
    //接收从机的应答 
    SOFTI2C_SDA_1(s); 
    SOFTI2C_NOP(s);
    SOFTI2C_SCL_1(s);
    SOFTI2C_NOP(s);   
    
    if(SOFTI2C_SDA_STATE(s))
    {
        SOFTI2C_SCL_0(s);
        SOFTI2C_SDA_1(s);

        return s->State = SOFTI2C_NACK;
    }
    else
    {
        SOFTI2C_SCL_0(s);
        SOFTI2C_SDA_1(s);
        
        return s->State = SOFTI2C_ACK;  
    }    
}

/* --------------------------------------------------------------------------*/
/** 
* @Brief:  SOFTI2C_ReceiveByte 
* 
* @Returns:   
*/
/* --------------------------------------------------------------------------*/
unsigned char SOFTI2C_ReceiveByte(SOFTI2C* s)
{
    unsigned char i = 0,Dat = 0;
    SOFTI2C_SDA_1(s);
    SOFTI2C_SCL_0(s); 

    for(i=0;i<8;i++)
    {
        SOFTI2C_SCL_1(s);//产生时钟上升沿[正脉冲],让从机准备好数据 
        SOFTI2C_NOP(s); 
        Dat<<=1;
        if(SOFTI2C_SDA_STATE(s)) //读引脚状态
        {
            Dat|=0x01; 
        }   
        SOFTI2C_SCL_0(s);//准备好再次接收数据  
        SOFTI2C_NOP(s);//等待数据准备好         
    }
    return Dat;
}
