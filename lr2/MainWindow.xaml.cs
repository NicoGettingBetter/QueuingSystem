/// <summary>
/// queuing system
/// </summary>
namespace Lr2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using win = System.Windows.Forms;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private double factor = 10;
        private int delta_t = new int();
        private List<TimeSpan> list_time_in_sys = new List<TimeSpan>();
        private int numOfApplicant = 0;
        private TimeSpan timeOfWaiting = new TimeSpan(0, 0, 0, 0, 10);
        private TimeSpan[] oldTimes = new TimeSpan[2] { new TimeSpan(), new TimeSpan() };
        private win.Timer timerQueue = new win.Timer();
        public event Action<int> pbFinishedEvent;
        private Item[] in_proc = new Item[2];
        private event Action<int> addedNewItemToQueueEvent;

        // for characteristics
        private List<TimeItem> pushTimeItem = new List<TimeItem>();
        private List<Tuple<TimeSpan, TimeSpan>> popTimeItem = new List<Tuple<TimeSpan, TimeSpan>>();
        private List<int>[] lengthOfQueue = new List<int>[2] { new List<int>(), new List<int>()};
        private double[] downtime = new double[2];
        private int[] unacceptedBids = new int[2];
        private double[] punacceptedBids = new double[2];
        private double[] max_t_in_q = new double[2];
        private double[] max_t_in_proc = new double[2];
        private string inProc0;
        private string inProc1;
        private string avInSysStr = "0";
        private string avLenQ0 = "0";
        private string avLenQ1 = "0";
        private string maxLenQ0 = "0";
        private string maxLenQ1 = "0";
        private string downtP0 = "0";
        private string downtP1 = "0";
        private string uacBids0 = "0";
        private string uacBids1 = "0";
        private string timeInQ0 = "0";
        private string timeInQ1 = "0";
        private string timeInProc0 = "0";
        private string timeInProc1 = "0";

        public string InProc0
        {
            set
            {
                inProc0 = value;
                OnPropertyChanged(nameof(inProc0));
            }
            get
            {
                return inProc0;
            }
        }

        public string InProc1
        {
            set
            {
                inProc1 = value;
                OnPropertyChanged(nameof(inProc1));
            }
            get
            {
                return inProc1;
            }
        }

        public string AvInSysStr
        {
            set
            {
                avInSysStr = value;
                OnPropertyChanged(nameof(avInSysStr));
            }
            get
            {
                return avInSysStr;
            }
        }

        public string AvLenQ0
        {
            set
            {
                avLenQ0 = value;
                OnPropertyChanged(nameof(avLenQ0));
            }
            get
            {
                return avLenQ0;
            }
        }

        public string AvLenQ1
        {
            set
            {
                avLenQ1 = value;
                OnPropertyChanged(nameof(avLenQ1));
            }
            get
            {
                return avLenQ1;
            }
        }

        public string MaxLenQ0
        {
            set
            {
                maxLenQ0 = value;
                OnPropertyChanged(nameof(maxLenQ0));
            }
            get
            {
                return maxLenQ0;
            }
        }

        public string MaxLenQ1
        {
            set
            {
                maxLenQ1 = value;
                OnPropertyChanged(nameof(maxLenQ1));
            }
            get
            {
                return maxLenQ1;
            }
        }

        public string DowntP0
        {
            set
            {
                downtP0 = value;
                OnPropertyChanged(nameof(downtP0));
            }
            get
            {
                return downtP0;
            }
        }

        public string DowntP1
        {
            set
            {
                downtP1 = value;
                OnPropertyChanged(nameof(downtP1));
            }
            get
            {
                return downtP1;
            }
        }

        public string UacBids0
        {
            set
            {
                uacBids0 = value;
                OnPropertyChanged(nameof(uacBids0));
            }
            get
            {
                return uacBids0;
            }
        }

        public string UacBids1
        {
            set
            {
                uacBids1 = value;
                OnPropertyChanged(nameof(uacBids1));
            }
            get
            {
                return uacBids1;
            }
        }

        public string TimeInQ0
        {
            set
            {
                timeInQ0 = value;
                OnPropertyChanged(nameof(timeInQ0));
            }
            get
            {
                return timeInQ0;
            }
        }

        public string TimeInQ1
        {
            set
            {
                timeInQ1 = value;
                OnPropertyChanged(nameof(timeInQ1));
            }
            get
            {
                return timeInQ1;
            }
        }

        public string TimeInProc0
        {
            set
            {
                timeInProc0 = value;
                OnPropertyChanged(nameof(timeInProc0));
            }
            get
            {
                return timeInProc0;
            }
        }

        public string TimeInProc1
        {
            set
            {
                timeInProc1 = value;
                OnPropertyChanged(nameof(timeInProc1));
            }
            get
            {
                return timeInProc1;
            }
        }

        //time 
        private win.Timer timer = new win.Timer();
        private TimeSpan oldTime = new TimeSpan();
        
        private TimeSpan stopWatch = new TimeSpan();
        public TimeSpan StopWatch
        {
            get
            {
                return stopWatch;
            }
            set
            {
                stopWatch = value;
                OnPropertyChanged(nameof(stopWatch));
            }
        }
        
        private Queue[] queue = new Queue[2];
        public double PBValue0
        {
            get
            {
                return pbValue0;
            }
            set
            {
                pbValue0 = (int)value;
                OnPropertyChanged(nameof(pbValue0));
            }
        }
        private double pbValue0;
        public double PBValue1
        {
            get
            {
                return pbValue1;
            }
            set
            {
                pbValue1 = (int)value;
                OnPropertyChanged(nameof(pbValue1));
            }
        }
        private double pbValue1;
        public double PBMaxValue0
        {
            get
            {
                return pbMaxValue0;
            }
            set
            {
                pbMaxValue0 = (int)value;
                OnPropertyChanged(nameof(pbMaxValue0));
            }
        }
        private double pbMaxValue0;
        public double PBMaxValue1
        {
            get
            {
                return pbMaxValue1;
            }
            set
            {
                pbMaxValue1 = (int)value;
                OnPropertyChanged(nameof(pbMaxValue1));
            }
        }
        private double pbMaxValue1;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            timer.Interval = 10;
            initializing();

            addedNewItemToQueueEvent += addedItemToQueue;
            pbFinishedEvent += pbFinished;
        }

        //------------------------------------------------------------------------------------
        // random number generators
        private double uniform(double a, double b)
        {
            Random r = new Random();
            return a + r.NextDouble() * (b - a);
        }

        private double normal(double a, double t)
        {
            const int k = 18;
            Random r = new Random();
            double sum = 0;
            for (int j = 0; j < k; ++j)
            {
                sum += r.NextDouble() - 0.5;
            }
            return a + Math.Sqrt(12.0 / k) * sum * t;
        }
        // random number generators
        //------------------------------------------------------------------------------------
        // queue

        private void initializing()
        {
            addColumnToDataGrid("Num", 46, queue_0);
            addColumnToDataGrid("Priority", 20, queue_0);
            addColumnToDataGrid("Num", 46, queue_1);
            addColumnToDataGrid("Priority", 20, queue_1);
            addColumnToDataGrid("time", 100, log);
            addColumnToDataGrid("log", 380, log);

            startQueue();
        }

        private void addColumnToDataGrid(string name, int width, DataGrid datagrid)
        {
            DataGridTextColumn c = new DataGridTextColumn();
            c.Header = name;
            c.Binding = new Binding(name);
            c.Width = width;
            datagrid.Columns.Add(c);
        }
        

        private void startQueue()
        {
            queue[0] = new Queue(queue_0, int.Parse(q_max.Text));
            queue[1] = new Queue(queue_1, int.Parse(q_max.Text));
            PBValue0 = 0;            
            PBValue1 = 0;            
            PBMaxValue0 = 10;
            PBMaxValue1 = 10;
        }

        public void addedItemToQueue(int numOfQueue)
        {
            lengthOfQueue[numOfQueue].Add(queue[numOfQueue].count());
            if (in_proc[numOfQueue] == null)
                startpb(numOfQueue);
            if (to_event.IsChecked == true)
                eventHappened();
        }

        public void startpb(int numOfQueue)
        {
            if (with_time.IsChecked == true)
            {
                timerQueue.Interval = delta_t;
                timerQueue.Start();
                timerQueue.Tick += new EventHandler(tickTimeQueue);
            }
            oldTimes[numOfQueue] = stopWatch;
            if (numOfQueue == 0)
            {
                in_proc[0] = queue[0].shift();
                InProc0 = in_proc[0].Num;
                PBMaxValue0 = (int)(normal(double.Parse(st_0_a.Text.Replace('.', ',')), double.Parse(st_0_t.Text.Replace('.', ','))) * 1000);
                if (PBMaxValue0 > max_t_in_proc[0] * 1000)
                    max_t_in_proc[0] = PBMaxValue0 / 1000.0;
                TimeInProc0 = string.Format("{0:n}", max_t_in_proc[0]);
            }
            else
            {
                in_proc[1] = queue[1].shift(); 
                InProc1 = in_proc[1].Num;
                PBMaxValue1 = (int)(normal(double.Parse(st_1_a.Text.Replace('.', ',')), double.Parse(st_1_t.Text.Replace('.', ','))) * 1000);
                if (PBMaxValue1 > max_t_in_proc[1] * 1000)
                    max_t_in_proc[1] = PBMaxValue1 / 1000.0;
                TimeInProc1 = string.Format("{0:n}", max_t_in_proc[1]);
            }            
            log.Items.Add(new LogItem((stopWatch.TotalMilliseconds / 1000).ToString(), string.Format("Заявка {0} заходит в {1} обработчик", in_proc[numOfQueue].Num, numOfQueue + 1)));
        }

        public void pbFinished(int numOfQueue)
        {
            oldTimes[numOfQueue] = stopWatch;
            log.Items.Add(new LogItem((stopWatch.TotalMilliseconds / 1000).ToString(), string.Format("Заявка {0} была обработана в {1} обработчике", in_proc[numOfQueue].Num, numOfQueue + 1)));
            if (numOfQueue == 0)
            {
                PBValue0 = 0;
                PBMaxValue0 = (int)(normal(double.Parse(st_0_a.Text.Replace('.', ',')), double.Parse(st_0_t.Text.Replace('.', ','))) * 1000);
                if (PBMaxValue0 > max_t_in_proc[0] * 1000)
                    max_t_in_proc[0] = PBMaxValue0 / 1000.0;
                TimeInProc0 = string.Format("{0:n}", max_t_in_proc[0]);
                if (queue[1].full)
                {
                    ++unacceptedBids[1];
                }
                else
                    queue[1].push(in_proc[0].Num, in_proc[0].Priority);
                log.Items.Add(new LogItem((StopWatch.TotalMilliseconds/1000).ToString(), string.Format("Заявка {0} добавлена в 2 очередь", in_proc[0].Num)));
                addedNewItemToQueueEvent(1);
                pushTimeItem.Find(x => x.item.Num == in_proc[0].Num).begin2 = StopWatch;
                if (queue[0].empty)
                {
                    in_proc[0] = null;
                    InProc0 = string.Empty;
                }
                else
                {
                    in_proc[0] = queue[0].shift();
                    InProc0 = in_proc[0].Num;
                    var temp = (StopWatch.TotalMilliseconds - pushTimeItem.Find(x => x.item.Num == in_proc[0].Num).begin.TotalMilliseconds)/1000.0;
                    if (temp > max_t_in_q[0])
                        max_t_in_q[0] = temp;
                    TimeInQ0 = string.Format("{0:n}", max_t_in_q[0]);
                    log.Items.Add(new LogItem((stopWatch.TotalMilliseconds / 1000).ToString(), string.Format("Заявка {0} заходит в {1} обработчик", in_proc[numOfQueue].Num, numOfQueue + 1)));
                }
            }
            else
            {
                PBValue1 = 0;
                PBMaxValue0 = (int)(normal(double.Parse(st_1_a.Text.Replace('.', ',')), double.Parse(st_1_t.Text.Replace('.', ','))) * 1000);
                if (PBMaxValue1 > max_t_in_proc[1] * 1000)
                    max_t_in_proc[1] = PBMaxValue1 / 1000.0;
                TimeInProc1 = string.Format("{0:n}", max_t_in_proc[1]);
                
                foreach (var i in (from j in pushTimeItem where j.item.Num == in_proc[1].Num select j))
                {
                    pushTimeItem.Remove(i);
                    popTimeItem.Add(new Tuple<TimeSpan, TimeSpan>(i.begin, stopWatch));
                    break;
                }
                if (queue[1].empty)
                {
                    in_proc[1] = null;
                    InProc1 = string.Empty;
                }
                else
                {
                    in_proc[1] = queue[1].shift();
                    InProc1 = in_proc[1].Num;
                    var temp = (stopWatch.TotalMilliseconds - pushTimeItem.Find(x => x.item.Num == in_proc[1].Num).begin2.TotalMilliseconds) / 1000.0;
                    if (temp > max_t_in_q[1])
                        max_t_in_q[1] = temp;
                    TimeInQ1 = string.Format("{0:n}", max_t_in_q[1]);
                    log.Items.Add(new LogItem((stopWatch.TotalMilliseconds / 1000).ToString(), string.Format("Заявка {0} заходит в {1} обработчик", in_proc[numOfQueue].Num, numOfQueue + 1)));
                }
            }
            lengthOfQueue[numOfQueue].Add(queue[numOfQueue].count());
            if (to_event.IsChecked == true)
                eventHappened();
        }
        
        private void tickTimeQueue(object sender, EventArgs e)
        {
            refresh(0);
            refresh(1);
        }

        private void refresh(int numOfQueue)
        {
            if (in_proc[numOfQueue] != null)
                queue[numOfQueue].Refresh();
        }

        private void eventHappened()
        {
            refresh(0);
            refresh(1);
        }

        // queue
        //------------------------------------------------------------------------------------
        // time
        

        private void tickTime(object sender, EventArgs e)
        {
            if (fact.Value > 20)
                factor = fact.Value - 20;
            else
                factor = fact.Value / 20;
            StopWatch += new TimeSpan(0, 0, 0, 0, (int)(factor * 10));
            TimeSpan ts = StopWatch;
            if (ts >= timeOfWaiting + oldTime)
            {
                timeOfWaiting = new TimeSpan(0, 0, 0, 0, (int)(1000 * uniform(double.Parse(g_a.Text.Replace('.', ',')), double.Parse(g_b.Text.Replace('.', ',')))));
                oldTime = ts;
                ++numOfApplicant;
                Random r = new Random();
                int rr = r.Next(1, 10);
                if (queue[0].full)
                {
                    ++unacceptedBids[0];
                    log.Items.Add(new LogItem((StopWatch.TotalMilliseconds / 1000).ToString(), string.Format("Заявка # {0} была отклонена", numOfApplicant)));
                }
                else
                {
                    queue[0].push(string.Format("# {0}", numOfApplicant), rr);
                    log.Items.Add(new LogItem((StopWatch.TotalMilliseconds / 1000).ToString(), string.Format("Заявка # {0} была добавлена в 1 очередь", numOfApplicant)));
                    addedNewItemToQueueEvent(0);
                    pushTimeItem.Add(new TimeItem(new Item(string.Format("# {0}", numOfApplicant), rr), StopWatch));
                }
            }
            if (in_proc[0] != null)
            {
                ts = StopWatch - oldTimes[0];
                PBValue0 = (int)ts.TotalMilliseconds;
                if (PBValue0 >= PBMaxValue0)
                    pbFinishedEvent(0);
            }
            else
                downtime[0] += factor * 10;
            if (in_proc[1] != null)
            {
                ts = StopWatch - oldTimes[1];
                PBValue1 = (int)ts.TotalMilliseconds;
                if (PBValue1 >= PBMaxValue1)
                    pbFinishedEvent(1);
            }
            else
                downtime[1] += factor * 10;
            DowntP0 = string.Format("{0:P}", downtime[0] / StopWatch.TotalMilliseconds);
            DowntP1 = string.Format("{0:P}", downtime[1] / StopWatch.TotalMilliseconds);
            if (popTimeItem.Count > 0)
            {
                AvInSysStr = string.Format("{0:N3} c", popTimeItem.Average(item => item.Item2.TotalMilliseconds - item.Item1.TotalMilliseconds)/1000);
            }
            if (lengthOfQueue[0].Count > 0)
            {
                AvLenQ0 = string.Format("{0:N}", lengthOfQueue[0].Average());
                MaxLenQ0 = string.Format("{0:N0}", lengthOfQueue[0].Max());
            }
            if (lengthOfQueue[1].Count > 0)
            {
                AvLenQ1 = string.Format("{0:N}", lengthOfQueue[1].Average());
                MaxLenQ1 = string.Format("{0:N0}", lengthOfQueue[1].Max());
            }
            punacceptedBids[0] = ((double)unacceptedBids[0]) / numOfApplicant;
            punacceptedBids[1] = ((double)unacceptedBids[1]) / (numOfApplicant - unacceptedBids[0]);
            UacBids0 = string.Format("{0:P}", punacceptedBids[0]);
            UacBids1 = string.Format("{0:P}", punacceptedBids[1]);
        }
        // time
        //------------------------------------------------------------------------------------
        // buttons
        private void start_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(delta_time.Text, out delta_t);            
            timer.Start();
            timer.Tick += new EventHandler(tickTime);
            timerQueue.Start();
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            if (with_time.IsChecked == true)
                timerQueue.Stop();

            timer.Tick -= new EventHandler(tickTime);
        }
        // buttons
        //----------------------------------------------------------------
        // classes        
        public class Queue
        {
            public Queue(DataGrid q, int length)
            {
                elems = new Item[length];
                this.q = q;
            }
            public Item[] elems;
            public int first = 0, last = 0;
            public bool full = false, empty = true;
            public DataGrid q { get; set; }
            public int next(int num)
            {
                ++num;
                if (num == elems.Length)
                    num = 0;
                return num;
            }
            public int prev(int num)
            {
                --num;
                if (num == -1)
                    num = elems.Length - 1;
                return num;
            }
            public void push(string el, int pr)
            {
                if (!full)
                {
                    var item = new Item(el, pr);
                    if (empty)
                    {
                        elems[last] = item;
                        last = next(last);
                    }
                    else
                    {
                        for (int i = last; ; i = prev(i))
                        {
                            if (i == first)
                            {
                                elems[i] = item;
                                break;
                            }
                            else if (item.Priority < elems[prev(i)].Priority)
                                elems[i] = elems[prev(i)];
                            else
                            {
                                elems[i] = item;
                                break;
                            }
                        }
                        last = next(last);
                    }
                    if (first == last)
                        full = true;
                    empty = false;
                }
            }
            public Item shift()
            {
                if (!empty)
                {
                    full = false;
                    if (next(first) == last)
                        empty = true;
                    Item i = elems[first];
                    elems[first] = null;
                    first = next(first);
                    return i;
                }
                return null;
            }
            public int count()
            {
                if (empty)
                    return 0;
                if (last > first)
                    return last - first;
                return elems.Length - first + last;
            }
            public int numInQueue(int i)
            {
                int j = first, k = 0;
                while (j != i)
                {
                    j = next(j);
                    ++k;
                }
                return k;
            }
            public void removeAll()
            {
                while (shift() != null) ;
            }
            public void Refresh()
            {
                while (q.Items.Count > 0)
                    q.Items.RemoveAt(0);
                if (!empty)
                    q.Items.Add(elems[first]);
                for (int i = next(first); !empty && i != last; i = next(i))
                {
                    q.Items.Add(elems[i]);
                }
            }
        }
        public class Item
        {
            public Item(string s, int p)
            {
                Num = s;
                Priority = p;
            }
            public string Num { get; set; }
            public int Priority { set; get; }
        }
        public class LogItem
        {
            public string log { set; get; }
            public string time { set; get; }
            public LogItem(string t, string l)
            {
                log = l;
                time = t;
            }
        }
        public class TimeItem
        {
            public Item item { set; get; }
            public TimeSpan begin { set; get; }
            public TimeSpan begin2 { set; get; }
            public TimeItem(Item i, TimeSpan b)
            {
                item = i;
                begin = b;
                begin2 = new TimeSpan();
            }
        }
        // classes
    }
}
