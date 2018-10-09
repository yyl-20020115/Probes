#include "AMMeter.h"
#include "main.h"
#include "stm32f4_discovery.h"

int AMMeterAck = FALSE;

AMMeterInfo InfoData = {0};

void AMMeterSetAck(void)
{
	AMMeterAck = TRUE;
}

int AMMeterIsAckReceived(void)
{
	return AMMeterAck;
}

void AMMeterClearAck(void)
{
	AMMeterAck = FALSE;
}

AMMeterInfo* AMMeterGetInfoData(void)
{
	return &InfoData;
}



int AMMeterInputFrom(USART_TypeDef *USART)
{
	while (USART_GetFlagStatus(USART, USART_FLAG_RXNE) == RESET)
	{
		
	}

	return (int)USART_ReceiveData(USART);
}

void AMMeterOutputTo(USART_TypeDef *USART, unsigned char data)
{
	USART_SendData(USART, data);

	while (USART_GetFlagStatus(USART, USART_FLAG_TC) == RESET)
	{
		
	}
}

int AMMeterSendCommand(unsigned char CommandChar,unsigned char Parameter)
{
	unsigned short CHECK_SUM = 0;
	if(  CommandChar == CMD_CONNECT 
		|| CommandChar == CMD_UNCONNECT 
	  || CommandChar == CMD_GET_INFO 
	  || CommandChar == CMD_SET_DP_SEL 
	  || CommandChar == CMD_SET_SPS 
	  || CommandChar == CMD_SET_BAUD 
	  || CommandChar == CMD_GET_DATA 
	  || CommandChar == CMD_SET_RANGE)
	{
		AMMeterOutputTo(AM_USART,HEAD_FIRST);
		AMMeterOutputTo(AM_USART,HEAD_SECOND);
		
		switch(CommandChar)
		{
			case CMD_CONNECT:
			{
				AMMeterOutputTo(AM_USART,0x02);
				AMMeterOutputTo(AM_USART,CMD_CONNECT);
				CHECK_SUM = 0x02+ CMD_CONNECT;
			}
			break;
			case CMD_UNCONNECT:
			{
				AMMeterOutputTo(AM_USART,0x02);
				AMMeterOutputTo(AM_USART,CMD_UNCONNECT);
				CHECK_SUM = 0x02+ CMD_UNCONNECT;
			}
			break;
			case CMD_GET_INFO:
			{
				AMMeterOutputTo(AM_USART,0x02);
				AMMeterOutputTo(AM_USART,CMD_GET_INFO);
				CHECK_SUM = 0x02+ CMD_GET_INFO;
			}
			break;
			case CMD_SET_DP_SEL:
			{
				AMMeterOutputTo(AM_USART,0x03);
				AMMeterOutputTo(AM_USART,CMD_SET_DP_SEL);
				AMMeterOutputTo(AM_USART,Parameter); //dp
				CHECK_SUM = 0x03+ CMD_SET_DP_SEL + Parameter;
			}
			break;
			case CMD_SET_SPS:
			{
				AMMeterOutputTo(AM_USART,0x03);
				AMMeterOutputTo(AM_USART,CMD_SET_SPS);
				AMMeterOutputTo(AM_USART,Parameter); //sps
				CHECK_SUM = 0x03+ CMD_SET_SPS + Parameter;
			}
			break;
			case CMD_SET_BAUD:
			{
				AMMeterOutputTo(AM_USART,0x03);
				AMMeterOutputTo(AM_USART,CMD_SET_BAUD);
				AMMeterOutputTo(AM_USART,Parameter); //baud
				CHECK_SUM = 0x03+ CMD_SET_BAUD + Parameter;
			}
			break;
			case CMD_GET_DATA:
			{
				AMMeterOutputTo(AM_USART,0x02);
				AMMeterOutputTo(AM_USART,CMD_GET_DATA);
				CHECK_SUM = 0x02+ CMD_GET_DATA;
			}
			break;
			case CMD_SET_RANGE:
			{
				AMMeterOutputTo(AM_USART,0x03);
				AMMeterOutputTo(AM_USART,CMD_SET_RANGE);
				AMMeterOutputTo(AM_USART,Parameter); //rr
				CHECK_SUM = 0x03+ CMD_SET_RANGE + Parameter;
			}
			break;			
		}
		
		//checksum
		AMMeterOutputTo(AM_USART,(unsigned char)((CHECK_SUM & 0xFF00)>>8));
		AMMeterOutputTo(AM_USART,(unsigned char)(CHECK_SUM & 0xFF));

		
		return TRUE;
	}
	else{
		return FALSE;
	}
}

int AMMeterParseGetDataReturnData(unsigned char* Buffer, unsigned char Length,unsigned short* Value)
{
	//AA 55 04 F6 v0 v1 XX XX
	if(Buffer!=0 && Length>=8)
	{
		if(Buffer[0] == HEAD_FIRST
		&& Buffer[1] == HEAD_SECOND
		&& Buffer[2] == 0x04
		&& Buffer[3] == CMD_RET_DATA
		&& (((unsigned short)Buffer[6]<<8) | Buffer[7]) 
			== (unsigned short)Buffer[2]
				+(unsigned short)Buffer[3]
				+(unsigned short)Buffer[4]
				+(unsigned short)Buffer[5]
		)
		{
			*Value = (unsigned short)Buffer[4] | (((unsigned short)Buffer[5]) << 8);
			
			return TRUE;
		}
	}
	return FALSE;
}

int AMMeterParseGetInfoReturnData(unsigned char* Buffer, unsigned char Length,unsigned char* Range, unsigned char* Class, unsigned int* ID)
{
	if(Buffer!=0 && Length>=12)
	{
		//AA 55 08 F5 rr cc a0 a1 a2 a3 XX XX
		if(Buffer[0] == HEAD_FIRST
			&& Buffer[1] == HEAD_SECOND
		&& Buffer[2] == 0x08
		&& Buffer[3] == CMD_RET_INFO
		&& ((unsigned short)Buffer[10]<<8 | Buffer[11]) ==
			(unsigned short)Buffer[2] 
		+ (unsigned short)Buffer[3]
		+ (unsigned short)Buffer[4]
		+ (unsigned short)Buffer[5]
		+ (unsigned short)Buffer[6]
		+ (unsigned short)Buffer[7]
		+ (unsigned short)Buffer[8]
		+ (unsigned short)Buffer[9]
			)
		{
			
			*Range = Buffer[4];
			*Class = Buffer[5];
			
			*ID = (unsigned int)Buffer[6] | (unsigned int)Buffer[7] <<8 | (unsigned int)Buffer[8] << 16 | (unsigned int)Buffer[9] <<24;
					
			return TRUE;
		}
			
		
	}
	return FALSE;
}

int AMMeterIsAckReturnData(unsigned char* Buffer,unsigned char Length)
{
	if(Buffer!=0 && Length>=6)
	{
		return Buffer[0] == HEAD_FIRST
			&& Buffer[1] == HEAD_SECOND
			&& Buffer[2] == 0x02
			&& Buffer[3] == CMD_ACK
			&& Buffer[4] == 0x00
			&& Buffer[5] == Buffer[2]+Buffer[3];
	}
	return FALSE;
}




