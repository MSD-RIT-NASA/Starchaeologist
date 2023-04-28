clc
clear

path = "C:\Users\p2201\OneDrive\Desktop\PLANET_Time\GPBA\External\Server\Planet Skeleton Data/1682568440/";
calibrationtime = 5.0;
deadtime = 1.08581;
softleft_v2 = 1.5;
hardleft_v2 = 3.0;
softforward_v2 = 7.5;
hardforward_v2 = 15.0;

[score, linegraphLR_X, linegraphLR_Y, BarGraphLR_EL, BarGraphLR_ML, BarGraphLR_C,BarGraphLR_MR,BarGraphLR_ER,linegraphFB_X,linegraphFB_Y,BarGraphFB_EF,BarGraphFB_MF,BarGraphFB_C,BarGraphFB_MB,BarGraphFB_EB]  = Planets_Analysis_Plot(path,calibrationtime,deadtime,softleft_v2, hardleft_v2);