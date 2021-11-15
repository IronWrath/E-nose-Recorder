clc; close all; clear all;

files = dir(fullfile('*.csv'));

dataset = zeros(size(files,1), 10);
dataset2 = zeros(size(files,1), 10);
labels = strings(size(files,1), 1);

idx = 1;
% for every csv file in current directory
for i = 1:size(files, 1)
    % read it in
    odour = load(fullfile(strcat(files(i).name)));   
    % remove time from file
    time = odour(:, 1);
    odour(:, 1) = [];
    odourSeconds = zeros(600, size(dataset, 2));
    
    for j = 1:size(odourSeconds, 1)
        odourSeconds(j, :) = nanmean(odour(time >= j-1 & time < j, :));
    end
    % Remove NaNs
    odourSeconds(isnan(odourSeconds(:, 1)), :) = [];
    % Remove duplicate columns
    odourSeconds(odourSeconds(:, 1) == odourSeconds(:, 2), :) = [];
   
    odour = movmean(odourSeconds, 3);
    dataset(idx, :) = median(odour);
    name = strsplit(files(i).name, '_');
    labels(idx) = name{1};
    % As we are might need to ignore certain recordings later (i.e., background)
    % keep track of the index
    idx = idx + 1;
    
    figure; hold on;
    for j = 1:10
        plot(1:size(odour, 1), odour(:, j), 'LineWidth', 2.5);
    end
    legend({'Air Quality', 'Air Quality Slope', 'Temperature', 'Pressure', 'Humidity', 'Gas', 'MQ3', 'MQ5', 'MQ9', 'HCHO'}, 'Location', 'northeastoutside');
    title(labels(idx-1));
    set(gca, 'FontSize', 15, 'FontWeight', 'Bold');
end