﻿//CppSensors
#include "pch.h"
#include "CppSensors.h"
#include "Math.h"

using namespace CppSensors;
using namespace Platform;


//ACCELEROMTER & GYROSCOPE
CppAccGyro::CppAccGyro()
{
	this->acc = Accelerometer::GetDefault();
	this->acc->ReportInterval = 10;
	this->acc->ReadingChanged += ref new TypedEventHandler < Accelerometer ^, AccelerometerReadingChangedEventArgs ^>(this, &CppAccGyro::accChanged);
	this->gyro = Gyrometer::GetDefault();
	this->gyro->ReportInterval = 10;
	this->gyro->ReadingChanged += ref new TypedEventHandler<Gyrometer ^, GyrometerReadingChangedEventArgs ^>(this, &CppAccGyro::gyroChanged);
	AccX = 50; 
	AccY = 50;
	AccZ = 50;
	GyrX = 0;
	GyrY = 0;
	GyrZ = 0;
}

void CppAccGyro::accChanged(Accelerometer ^sender, AccelerometerReadingChangedEventArgs ^args)
{
	AccX = args->Reading->AccelerationX;
	AccY = args->Reading->AccelerationY;
	AccZ = args->Reading->AccelerationZ;
	this->onReadingChanged(AccX, AccY, AccZ, GyrX, GyrY, GyrZ);
}

void CppAccGyro::gyroChanged(Gyrometer ^sender, GyrometerReadingChangedEventArgs ^args)
{
	GyrX = args->Reading->AngularVelocityX;
	GyrY = args->Reading->AngularVelocityX;
	GyrZ = args->Reading->AngularVelocityZ;
	this->onReadingChanged(AccX, AccY, AccZ, GyrX, GyrY, GyrZ);
}

CppAccGyro::~CppAccGyro()
{

}

//ACCELEROMETER
CppAcc::CppAcc()
{
	this->acc = Accelerometer::GetDefault();
	this->acc->ReportInterval = 10;
	this->acc->ReadingChanged += ref new TypedEventHandler < Accelerometer ^, AccelerometerReadingChangedEventArgs ^>(this, &CppAcc::accChanged);
}

void CppAcc::accChanged(Accelerometer ^sender, AccelerometerReadingChangedEventArgs ^args)
{
	this->onReadingChanged(args->Reading->AccelerationX, args->Reading->AccelerationY, args->Reading->AccelerationZ);
}

CppAcc::~CppAcc()
{

}

//GYROMETER
CppGyro::CppGyro()
{
	this->gyro = Gyrometer::GetDefault();
	this->gyro->ReportInterval = this->gyro->MinimumReportInterval;
	this->gyro->ReadingChanged += ref new TypedEventHandler<Gyrometer ^, GyrometerReadingChangedEventArgs ^>(this, &CppGyro::gyroChanged);
}

void CppGyro::gyroChanged(Gyrometer ^sender, GyrometerReadingChangedEventArgs ^args)
{
	this->onReadingChanged(args->Reading->AngularVelocityX, args->Reading->AngularVelocityY, args->Reading->AngularVelocityZ);
}

CppGyro::~CppGyro()
{

}

