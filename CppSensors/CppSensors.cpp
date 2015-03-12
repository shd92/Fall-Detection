﻿//CppSensors
#include "pch.h"
#include "CppSensors.h"
#include "Math.h"

using namespace CppSensors;
using namespace Platform;

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

//COMPASS 
CppCompass::CppCompass()
{
	this->compass = Compass::GetDefault();
	this->compass->ReportInterval = 10;
	this->compass->ReadingChanged += ref new TypedEventHandler <Compass ^, CompassReadingChangedEventArgs ^>(this, &CppCompass::compassChanged);
	n = 0;
}

void CppCompass::compassChanged(Compass ^sender, CompassReadingChangedEventArgs ^args)
{
	double nNew = args->Reading->HeadingMagneticNorth;

	//compassChanged() is just redirecting the function call from the Compass object to our C# class
	this->onReadingChanged(nNew);
}

double CppCompass::getN()
{
	return n;
}

CppCompass::~CppCompass()
{

}