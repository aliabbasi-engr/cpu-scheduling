using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CPU_Scheduling
{
    public partial class Form1 : Form
    {
        List<Result> resultsFCFS = new List<Result>();
        List<Result> resultsSJF = new List<Result>();
        List<Result> resultsRR = new List<Result>();
        List<Result> resultsP = new List<Result>();
        List<Result> resultsSRTF = new List<Result>();

        public Form1()
        {
            InitializeComponent();
        }

        private void BtnScheduleFCFS_Click(object sender, EventArgs e)
        {
            resultsFCFS.Clear();
            dgvFCFSOutput.Rows.Clear();
            dgvFCFSOutput.Refresh();
            var sum = 0;
            int currentTime = 0;
            var processes = new Queue<Tuple<string, int, int>>();

            for (int j = 0; j < dgvFCFSInput.RowCount - 1; j++)
                processes.Enqueue(new Tuple<string, int, int>(dgvFCFSInput[0, j].Value.ToString(), int.Parse(dgvFCFSInput[1, j].Value.ToString()), j));

            foreach (var process in processes)
            {
                var result = new Result
                {
                    Id = process.Item3,
                    Name = process.Item1,
                    StartTime = currentTime,
                    EndTime = currentTime + process.Item2
                };
                currentTime += process.Item2;
                resultsFCFS.Add(result);
            }
            currentTime = 0;

            foreach (var process in processes)
            {
                dgvFCFSOutput.Rows.Add(process.Item1, currentTime, currentTime + process.Item2, process.Item2);
                sum += currentTime;
                currentTime += process.Item2;
            }
            lblAvgWaitingTimeFCFS.Text = ((double)sum / processes.Count).ToString(".##");
        }

        private void BtnScheduleSJF_Click(object sender, EventArgs e)
        {
            resultsSJF.Clear();
            dgvSJFOutput.Rows.Clear();
            dgvSJFOutput.Refresh();
            var sum = 0;
            int currentTime = 0;
            var processes = new List<Tuple<string, int, int>>();

            for (int j = 0; j < dgvSJFInput.RowCount - 1; j++)
                processes.Add(new Tuple<string, int, int>(dgvSJFInput[0, j].Value.ToString(), int.Parse(dgvSJFInput[1, j].Value.ToString()), j));

            var sortedProcesses = processes.OrderBy(process => process.Item2).ToList();

            foreach (var process in sortedProcesses)
            {
                var result = new Result
                {
                    Id = process.Item3,
                    Name = process.Item1,
                    StartTime = currentTime,
                    EndTime = currentTime + process.Item2
                };
                currentTime += process.Item2;
                resultsSJF.Add(result);
            }
            currentTime = 0;

            foreach (var process in sortedProcesses)
            {
                dgvSJFOutput.Rows.Add(process.Item1, currentTime, currentTime + process.Item2, process.Item2);
                sum += currentTime;
                currentTime += process.Item2;
            }
            lblAvgWaitingTimeSJF.Text = ((double)sum / processes.Count).ToString(".##");
        }

        private void BtnScheduleRR_Click(object sender, EventArgs e)
        {
            resultsRR.Clear();
            dgvRROutput.Rows.Clear();
            dgvRROutput.Refresh();
            var sum = 0;
            int quantum = int.Parse(tbQuantum.Text);
            int currentTime = 0;

            var processes = new Queue<Tuple<string, int, int>>();
            for (int j = 0; j < dgvRRInput.RowCount - 1; j++)
                processes.Enqueue(new Tuple<string, int, int>(dgvRRInput[0, j].Value.ToString(), int.Parse(dgvRRInput[1, j].Value.ToString()), j));

            while (processes.Count != 0)
            {
                var process = processes.Dequeue();
                var duration = process.Item2 < quantum ? process.Item2 : quantum;
                dgvRROutput.Rows.Add(process.Item1, currentTime, currentTime + duration, duration);
                
                var result = new Result
                {
                    Id = process.Item3,
                    Name = process.Item1,
                    StartTime = currentTime,
                    EndTime = currentTime + duration
                };
                resultsRR.Add(result);
                sum += currentTime;
                currentTime += duration;
                
                if (process.Item2 - duration > 0)
                    processes.Enqueue(new Tuple<string, int, int>(process.Item1, process.Item2 - duration, process.Item3));
            }
            lblAvgWaitingTimeRR.Text = ((double)sum / dgvRROutput.Rows.Count - 1).ToString(".##");
        }

        private void BtnScheduleP_Click(object sender, EventArgs e)
        {
            resultsP.Clear();
            dgvPOutput.Rows.Clear();
            dgvPOutput.Refresh();
            var sum = 0;

            var currentTime = 0;
            var processes = new List<Tuple<string, int, int, int>>();
            for (int j = 0; j < dgvPInput.RowCount - 1; j++)
                processes.Add(new Tuple<string, int, int, int>(dgvPInput[0, j].Value.ToString(), int.Parse(dgvPInput[1, j].Value.ToString()), int.Parse(dgvPInput[2, j].Value.ToString()), j));

            var sortedProcesses = processes.OrderBy(process => process.Item3);

            foreach (var process in sortedProcesses)
            {
                var result = new Result
                {
                    Id = process.Item4,
                    Name = process.Item1,
                    StartTime = currentTime,
                    EndTime = currentTime + process.Item2
                };;
                currentTime += process.Item2;
                resultsP.Add(result);
            }
            currentTime = 0;

            foreach (var process in sortedProcesses)
            {
                dgvPOutput.Rows.Add(process.Item1, currentTime, currentTime + process.Item2, process.Item2, process.Item3);
                sum += currentTime;
                currentTime += process.Item2;
            }
            lblAvgWaitingTimeP.Text = ((double)sum / processes.Count).ToString(".##");
        }

        private void BtnScheduleSRTF_Click(object sender, EventArgs e)
        {
            resultsSRTF.Clear();
            dgvSRTFOutput.Rows.Clear();
            dgvSRTFOutput.Refresh();

            var processes = new List<Model>();
            FillProcesses(processes);

            var sumOfBurstTimes = processes.Select(process => process.BurstTime).Sum();
            for (var currentTime = 0; currentTime < sumOfBurstTimes; currentTime++)
            {
                var filteredProcesses = processes.Where(process => process.ArrivalTime <= currentTime).ToList();
                var min = filteredProcesses.Select(process => process.BurstTime).Min();
                var candidateProcess = filteredProcesses.Where(process => process.BurstTime == min).First();
                candidateProcess.BurstTime--;
                if(candidateProcess.BurstTime == 0)
                    processes.Remove(candidateProcess);

                if (resultsSRTF.Count == 0)
                {
                    resultsSRTF.Add(new Result
                    {
                        Id = candidateProcess.Id,
                        Name = candidateProcess.Name,
                        StartTime = currentTime,
                        EndTime = currentTime + 1
                    });
                }
                else
                {
                    var lastResult = resultsSRTF[resultsSRTF.Count - 1];
                    if (lastResult.Id == candidateProcess.Id)
                        lastResult.EndTime++;
                    else
                    {
                        resultsSRTF.Add(new Result
                        {
                            Id = candidateProcess.Id,
                            Name = candidateProcess.Name,
                            StartTime = currentTime,
                            EndTime = currentTime + 1
                        });
                    }
                }
            }

            // Calculate waiting times
            FillProcesses(processes);
            var waitingTimes = new List<double>();
            var ids = resultsSRTF.Select(result => result.Id).Distinct().ToList();
            foreach (var id in ids)
            {
                var arrivalTime = processes.Where(process => process.Id == id).First().ArrivalTime;
                var idProcesses = resultsSRTF.Where(result => result.Id == id).ToList();
                var waitingTime = idProcesses[0].StartTime - arrivalTime;
                for (var i = 1; i < idProcesses.Count; i++)
                    waitingTime += idProcesses[i].StartTime - idProcesses[i - 1].EndTime;
                waitingTimes.Add(waitingTime);
            }
            var avg = waitingTimes.Average();
            lblAvgWaitingTimeSRTF.Text = avg.ToString(".##");
        }

        private void FillProcesses(List<Model> processes)
        {
            processes.Clear();
            for (int j = 0; j < dgvSRTFInput.RowCount - 1; j++)
                processes.Add(new Model
                {
                    Id = j,
                    Name = dgvSRTFInput[0, j].Value.ToString(),
                    ArrivalTime = int.Parse(dgvSRTFInput[1, j].Value.ToString()),
                    BurstTime = int.Parse(dgvSRTFInput[2, j].Value.ToString())
                });
        }
        
        private void BtnClearFCFS_Click(object sender, EventArgs e)
        {
            dgvFCFSOutput.Rows.Clear();
            dgvFCFSOutput.Refresh();
            dgvFCFSInput.Rows.Clear();
            dgvFCFSInput.Refresh();
            lblAvgWaitingTimeFCFS.Text = "---";
        }

        private void BtnClearSJF_Click(object sender, EventArgs e)
        {
            dgvSJFOutput.Rows.Clear();
            dgvSJFOutput.Refresh();
            dgvSJFInput.Rows.Clear();
            dgvSJFInput.Refresh();
            lblAvgWaitingTimeSJF.Text = "---";
        }

        private void BtnClearRR_Click(object sender, EventArgs e)
        {
            dgvRROutput.Rows.Clear();
            dgvRROutput.Refresh();
            dgvRRInput.Rows.Clear();
            dgvRRInput.Refresh();
            lblAvgWaitingTimeRR.Text = "---";
        }

        private void BtnClearP_Click(object sender, EventArgs e)
        {
            dgvPOutput.Rows.Clear();
            dgvPOutput.Refresh();
            dgvPInput.Rows.Clear();
            dgvPInput.Refresh();
            lblAvgWaitingTimeP.Text = "---";
        }

        private void BtnClearSRTF_Click(object sender, EventArgs e)
        {
            dgvSRTFOutput.Rows.Clear();
            dgvSRTFOutput.Refresh();
            dgvSRTFInput.Rows.Clear();
            dgvSRTFInput.Refresh();
            lblAvgWaitingTimeSRTF.Text = "---";
        }

        private void BtnViewChartFCFS_Click(object sender, EventArgs e)
        {
            var chartForm = new ChartForm(resultsFCFS);
            chartForm.Show();
        }

        private void BtnViewChartSJF_Click(object sender, EventArgs e)
        {
            var chartForm = new ChartForm(resultsSJF);
            chartForm.Show();
        }

        private void BtnViewChartRR_Click(object sender, EventArgs e)
        {
            var chartForm = new ChartForm(resultsRR);
            chartForm.Show();
        }

        private void BtnViewChartP_Click(object sender, EventArgs e)
        {
            var chartForm = new ChartForm(resultsP);
            chartForm.Show();
        }

        private void BtnViewChartSRTF_Click(object sender, EventArgs e)
        {
            var chartForm = new ChartForm(resultsSRTF);
            chartForm.Show();
        }

        private void BtnCmp_Click(object sender, EventArgs e)
        {
            BtnClearFCFS_Click(sender, e);
            BtnClearSJF_Click(sender, e);
            BtnClearRR_Click(sender, e);
            BtnClearP_Click(sender, e);

            for (int j = 0; j < dgvCmpInput.RowCount - 1; j++)
            {
                dgvFCFSInput.Rows.Add(dgvCmpInput[0, j].Value, dgvCmpInput[1, j].Value);
                dgvSJFInput.Rows.Add(dgvCmpInput[0, j].Value, dgvCmpInput[1, j].Value);
                dgvRRInput.Rows.Add(dgvCmpInput[0, j].Value, dgvCmpInput[1, j].Value);
                tbQuantum.Text = tbCmpQuantum.Text;
                dgvPInput.Rows.Add(dgvCmpInput[0, j].Value, dgvCmpInput[1, j].Value, dgvCmpInput[1, j].Value);
            }

            BtnScheduleFCFS_Click(sender, e);
            BtnScheduleSJF_Click(sender, e);
            BtnScheduleRR_Click(sender, e);
            BtnScheduleP_Click(sender, e);

            lblFCFSAWT.Text = lblAvgWaitingTimeFCFS.Text;
            lblSJFAWT.Text = lblAvgWaitingTimeSJF.Text;
            lblRRAWT.Text = lblAvgWaitingTimeRR.Text;
            lblPAWT.Text = lblAvgWaitingTimeP.Text;

            var algorithms = new List<Tuple<string, double, int>>();

            algorithms.Add(new Tuple<string, double, int>("First Come First Serve", double.Parse(lblFCFSAWT.Text), int.Parse(dgvFCFSOutput[2, dgvFCFSOutput.RowCount - 1].Value.ToString())));
            algorithms.Add(new Tuple<string, double, int>("Shortest Job First", double.Parse(lblSJFAWT.Text), int.Parse(dgvSJFOutput[2, dgvSJFOutput.RowCount - 1].Value.ToString())));
            algorithms.Add(new Tuple<string, double, int>("Round Robin", double.Parse(lblRRAWT.Text), int.Parse(dgvRROutput[2, dgvRROutput.RowCount - 1].Value.ToString())));
            algorithms.Add(new Tuple<string, double, int>("Priority", double.Parse(lblPAWT.Text), int.Parse(dgvPOutput[2, dgvPOutput.RowCount - 1].Value.ToString())));

            lblTETFCFS.Text = algorithms.Where(name => name.Item1 == "First Come First Serve").ToList().First().Item3.ToString();
            lblTETSJF.Text = algorithms.Where(name => name.Item1 == "Shortest Job First").ToList().First().Item3.ToString();
            lblTETRR.Text = algorithms.Where(name => name.Item1 == "Round Robin").ToList().First().Item3.ToString();
            lblTETP.Text = algorithms.Where(name => name.Item1 == "Priority").ToList().First().Item3.ToString();

            var sortedAvgs = algorithms.OrderBy(value => value.Item2).ToList();

            lblBestAWT.Text = $"The {sortedAvgs[0].Item1} algorithm has the shortest average\n waiting time.({sortedAvgs[0].Item2} msec)";

            var sortedElpasedTimes = algorithms.OrderBy(value => value.Item3).ToList();

            lblBestTET.Text = $"The {sortedElpasedTimes[0].Item1} algorithm is the fastest algorithm\n for executing these processes.({sortedElpasedTimes[0].Item3} msec)";
        }

        private void BtnClearCmp_Click(object sender, EventArgs e)
        {
            dgvCmpInput.Rows.Clear();
            tbCmpQuantum.Clear();

            lblFCFSAWT.Text = "--";
            lblSJFAWT.Text = "--";
            lblRRAWT.Text = "--";
            lblPAWT.Text = "--";

            lblTETFCFS.Text = "--";
            lblTETSJF.Text = "--";
            lblTETRR.Text = "--";
            lblTETP.Text = "--";

            BtnClearFCFS_Click(sender, e);
            BtnClearSJF_Click(sender, e);
            BtnClearRR_Click(sender, e);
            BtnClearP_Click(sender, e);
        }
    }
}
    