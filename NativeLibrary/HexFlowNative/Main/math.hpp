#pragma once
#include "pch.h"

int modular(int a, int b) { return (a % b + b) % b; }

int floor_div(int a, int b) { return (a - modular(a, b)) / b; }
