function ave = simple(x, y)
    filename = 'External/Excel/OPC Data.xlsm';
    sheet = 1;
    xlRange = 'B4';
    xlswrite(filename,x,sheet,xlRange)
end