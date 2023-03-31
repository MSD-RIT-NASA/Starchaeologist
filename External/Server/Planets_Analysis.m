function [score, linegraphLR_X, linegraphLR_Y, BarGraphLR_EL, BarGraphLR_ML, BarGraphLR_C,BarGraphLR_MR,BarGraphLR_ER,linegraphFB_X,linegraphFB_Y,BarGraphFB_EF,BarGraphFB_MF,BarGraphFB_C,BarGraphFB_MB,BarGraphFB_EB]  = Planets_Analysis(path,calibrationtime,deadtime,softleft_v2, hardleft_v2, softforward_v2, hardforward_v2)


% clc
% clear
% 
% path = "C:\Users\jordo\OneDrive\Desktop\School\MSD\PLANETs\5_sec_calib, 5 seconds to get balanced on the board, 5 seconds at end throw out\1679409559.32121";
% 
% deadtime = 10;
% calibrationtime = 5;

% Setting the limits of left & right angles of person
% hardleft_v2 = 10;
% softleft_v2 = 5;
hardright_v2 = -1*hardleft_v2;
softright_v2 = -1*softleft_v2;
h_left_v2 = 0;
s_left_v2 = 0;
h_right_v2 = 0;
s_right_v2 = 0;
mid_roll = 0;

% hardforward = 20;
% softforward = 10;
hardback = -1*hardforward_v2;
softback = -1*softforward_v2;

h_forward = 0;
s_forward = 0;
h_back = 0;
s_back = 0;
mid_pitch = 0;

%Columns for XYZ
timecol = 1;
xcol = 2;
ycol = 3;

%Time X Y Z
%chest = readmatrix("head_position.csv"); %For the moment I am using head position, but calling it chest
chest = readmatrix(path+"\chest_position.csv");
lfoot = readmatrix(path+"\left_foot_position.csv");
rfoot = readmatrix(path+"\right_foot_position.csv");

%Pitch Yaw Roll (Along X Y Z respectively)
chest_pyr = readmatrix(path+"\chest_rotation.csv"); 

%Resizes the XYZ matrices to be the same size, deleting the beginning of
%each so that they can be used for slant calculations.
arrayresize = 0;
while arrayresize == 0
    checkchest = chest(1,1);
    checklfoot = lfoot(1,1);
    checkrfoot = rfoot(1,1);
        if ((checkchest < checklfoot) || (checkchest < checkrfoot))
            chest(1,:) = [];
        elseif ((checklfoot < checkchest) || (checklfoot < checkrfoot))
            lfoot(1,:) = [];
        elseif ((checkrfoot < checklfoot) || (checkrfoot < checkchest))
            rfoot(1,:) = [];
        else
            arrayresize = 1;
        end
end

    

%Reformatting the excels, not necessary anymore, but I wrote this at the
%beginning and was too lazy to change it.
chest_xy(:,1) = chest(:,timecol);
chest_xy(:,2) = chest(:,xcol);
chest_xy(:,3) = chest(:,ycol);

%Finds the average position of the feet
foot_xy(:,1) = lfoot(:,timecol);
for i=1:size(lfoot,1)
    foot_xy(i,2) = (lfoot(i,xcol)+rfoot(i,xcol))/2;
    foot_xy(i,3) = (lfoot(i,ycol)+rfoot(i,ycol))/2;
end

%calcs the slants using arctan and X&Y position. Does switch x&y so that
%there are no undefined values.
slant = zeros(size(chest_xy,1),1);
for i=1:size(chest_xy,1)
    slant(i) = (atan((chest_xy(i,2)-foot_xy(i,2))/(chest_xy(i,3)-foot_xy(i,3))))*180/pi(); %Angle of person, from verticle (mathmatically inverse)
    timercheck = chest_xy(i,1);
    if round(timercheck,2) == calibrationtime %Finds what row number is equivalent to 1 second
        timeEQoneS = i;
    end
end

%This section is to recalibrate the slant
recalS = zeros(size(timeEQoneS));
for i=1:timeEQoneS
    recalS(i) = slant(i);
end
averageOffS = mean(recalS);
slantoriginal = slant;
slant = slant - averageOffS;

for i=1:size(chest_xy,1)
    timercheck = chest_xy(i,1);
    if round(timercheck,2) == deadtime %Finds what row number is equivalent to 1 second
        timeDeadZone = i;
    end
end

chest_xy_orginal = chest_xy;
slantwithDead = slant;

for i=1:timeDeadZone
    slant(1) = [];
    chest_xy(1,:) = [];
end


for i=1:size(slant)
    if slant(i) <= hardright_v2
        h_right_v2 = h_right_v2 + 1;
    elseif slant(i) <= softright_v2
        s_right_v2 = s_right_v2 + 1;
    elseif slant(i) >= hardleft_v2
        h_left_v2 = h_left_v2 + 1;
    elseif slant(i) >= softleft_v2
        s_left_v2 = s_left_v2 + 1;
    else
        mid_roll = mid_roll+1;
    end
end



for i=1:size(chest_pyr,1)
    timercheck = chest_pyr(i,1);
    if round(timercheck,2) == calibrationtime %Finds what row number is equivalent to 1 second
        timeEQoneP = i;
    end
end
pitch = chest_pyr(:,2);
%This section is to recalibrate the pitch
recalP = zeros(size(timeEQoneP));
for i=1:timeEQoneP
    recalP(i) = pitch(i);
end
averageOffP = mean(recalP);
pitchoriginal = pitch;
pitch = pitch - averageOffP;

for i=1:size(chest_pyr,1)
    timercheck = chest_pyr(i,1);
    if round(timercheck,2) == deadtime %Finds what row number is equivalent to 1 second
        timeDeadZoneP = i;
    end
end

chest_pyr_original = chest_pyr;

pitchwithDead = pitch;
for i=1:timeDeadZoneP
    pitch(1) = [];
    chest_pyr(1,:) =[];
end

for i=1:size(pitch)
    if pitch(i) >= hardforward_v2
        h_forward = h_forward + 1;
    elseif pitch(i) >= softforward_v2
        s_forward = s_forward + 1;
    elseif pitch(i) <= hardback
        h_back = h_back + 1;
    elseif pitch(i) <= softback
        s_back = s_back + 1;
    else
        mid_pitch = mid_pitch+1;
    end
end

total_lean = h_left_v2+s_left_v2+mid_roll+s_right_v2+h_right_v2;
hRprop_v2 = h_right_v2/total_lean;
sRprop_v2 = s_right_v2/total_lean;
midprop = mid_roll/total_lean;
sLprop_v2 = s_left_v2/total_lean;
hLprop_v2 = h_left_v2/total_lean;

total_pitch = h_forward+s_forward+mid_pitch+s_back+h_back;
hFprop = h_forward/total_pitch;
sFprop = s_forward/total_pitch;
midPprop = mid_pitch/total_pitch;
sBprop = s_back/total_pitch;
hBprop = h_back/total_pitch;

placeholderLR = chest_xy(:,1);
linegraphLR_X = zeros(size(placeholderLR,2),1);

%for i=1:size(placeholderLR,1)
%    linegraphLR_X(1,i) = (placeholderLR(i) - deadtime);
%end

score = 100*midprop + 50*sRprop_v2+50*sLprop_v2 - 100*hRprop_v2-100*hLprop_v2;
linegraphLR_X = (chest_xy(:,1)-deadtime);
linegraphLR_Y = slant;
BarGraphLR_EL = hLprop_v2;
BarGraphLR_ML = sLprop_v2;
BarGraphLR_C = midprop;
BarGraphLR_MR = sRprop_v2;
BarGraphLR_ER = hRprop_v2;

placeholderFB = chest_pyr(:,1);
linegraphFB_X = zeros(size(placeholderFB,2),1);

%for i=1:size(placeholderFB,1)
%    linegraphFB_X(1,i) = (placeholderFB(i) - deadtime);
%end

%linegraphLR_X = ((linegraphLR_X));
%linegraphFB_X = ((linegraphFB_X));

linegraphFB_X = (chest_pyr(:,1)-deadtime);
linegraphFB_Y = pitch;
BarGraphFB_EF = hFprop;
BarGraphFB_MF = sFprop;
BarGraphFB_C = midPprop;
BarGraphFB_MB = sBprop;
BarGraphFB_EB = hBprop;



% anglevtime = figure;
% plot((chest_xy(:,1)-deadtime),slant);
% hold on
% grid on
% %ylim([(-1+hardright_v2) (1+hardleft_v2)])
% title('Left and Right Lean Over Time')
% ylabel('Left and Right Lean [degrees]')
% xlabel('Time [s]')
% xlim([(chest_xy(1,1)-deadtime) (chest_xy(size(chest_xy,1),1)-deadtime)])
% ylim([-1*max((1+max(abs(slant))),(hardleft_v2+2)) max((1+max(abs(slant))),(hardleft_v2+2))])
% 
% x_values_slant = chest_xy(:,1)-deadtime;
% SHardLineRight_Y = zeros(size(x_values_slant))+hardright_v2;
% plot(x_values_slant,SHardLineRight_Y,'Color',[0.6350 0.0780 0.1840])
% 
% SHardLineLeft_Y = zeros(size(x_values_slant))+hardleft_v2;
% plot(x_values_slant,SHardLineLeft_Y,'Color',[0.6350 0.0780 0.1840])
% 
% SSoftLineRight_Y = zeros(size(x_values_slant))+softright_v2;
% plot(x_values_slant,SSoftLineRight_Y,'Color',[0.9290 0.6940 0.1250])
% 
% SSoftLineLeft_Y = zeros(size(x_values_slant))+softleft_v2;
% plot(x_values_slant,SSoftLineLeft_Y,'Color',[0.9290 0.6940 0.1250])
% 
% saveas(gcf,'SlantOverTime.png')
% 
% hold off
% 
% 
% 
% bargraph = figure;
% %bar_x = ["Leaning Left" "Leaning Slightly Left" "Centered" "Leaning Slightly Right" "Leaning Right"];
% bar_x = categorical({'Extreme Left Lean','Slight Left Lean','Centered','Slight Right Lean','Extreme Right Lean'});
% bar_x = reordercats(bar_x,{'Extreme Left Lean','Slight Left Lean','Centered','Slight Right Lean','Extreme Right Lean'});
% bar_y = [hLprop_v2 sLprop_v2 midprop sRprop_v2 hRprop_v2];
% bar(bar_x,bar_y)
% title('Time Spent at Severity of Left & Right Lean')
% ylabel('Percentage of Time Spent')
% saveas(gcf,'SlantBar.png')
% 
% anglevtimePitch = figure;
% plot((chest_pyr(:,1)-deadtime),pitch);
% grid on
% hold on
% %ylim([(-1+hardback) (1+hardforward)])
% title('Foward and Backwards Lean Over Time')
% ylabel('Forward and Backwards Lean [degrees]')
% xlabel('Time [s]')
% xlim([(chest_pyr(1,1)-deadtime) (chest_pyr(size(chest_pyr,1))-deadtime)])
% ylim([-1*max((10+max(abs(pitch),(hardforward+10)))) max((10+max(abs(pitch),(hardforward+10))))])
% 
% x_values_pitch = chest_pyr(:,1)-deadtime; %It was working, then I noticed this was xy instead of pyr, I changed it, hopefully it does not break now
% SHardLineForw_Y = zeros(size(x_values_pitch))+hardforward;
% plot(x_values_pitch,SHardLineForw_Y,'Color',[0.6350 0.0780 0.1840])
% 
% SHardLineBack_Y = zeros(size(x_values_pitch))+hardback;
% plot(x_values_pitch,SHardLineBack_Y,'Color',[0.6350 0.0780 0.1840])
% 
% SSoftLineForw_Y = zeros(size(x_values_pitch))+softforward;
% plot(x_values_pitch,SSoftLineForw_Y,'Color',[0.9290 0.6940 0.1250])
% 
% SSoftLineBack_Y = zeros(size(x_values_pitch))+softback;
% plot(x_values_pitch,SSoftLineBack_Y,'Color',[0.9290 0.6940 0.1250])
% 
% saveas(gcf,'PitchOverTime.png')
% hold off
% 
% bargraphPitch = figure;
% %bar_x = ["Leaning Left" "Leaning Slightly Left" "Centered" "Leaning Slightly Right" "Leaning Right"];
% bar_xP = categorical({'Extreme Forward Lean','Slight Forward Lean','Centered','Slight Back Lean','Extreme Back Lean'});
% bar_xP = reordercats(bar_xP,{'Extreme Forward Lean','Slight Forward Lean','Centered','Slight Back Lean','Extreme Back Lean'});
% bar_yP = [hFprop sFprop midPprop sBprop hBprop];
% bar(bar_xP,bar_yP)
% title('Time Spent at Severity of Forward & Backwards Lean')
% ylabel('Percentage of Time Spent')
% saveas(gcf,'PitchBar.png')
end
