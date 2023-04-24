clc
clear

path = "C:\Users\p2201\OneDrive\Desktop\Sheridan-Test-Ground\GPBA\External\Server\data.txt";
GreenScore = 80;
YellowScore = 60;
OrangeScore = 40;

[CometX, CometY, R_score_rt, FinalScore, mat] = DataAnalysisPlot(path, GreenScore, YellowScore, OrangeScore);
