using CNTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;

namespace LSTMTimeSeriesDemo
{
    public partial class LSTMTimeSeries : Form
    {
        int inDim = 1;
        int ouDim = 1;
        int batchSize = 100;
        int timeStep = 1; //5
        string featuresName = "feature";
        string labelsName = "label";
        float koef = 0.2f;
        Func<double, double> func = Math.Sin;

        Dictionary<string, (float[][] train, float[][] valid, float[][] test)> DataSet;
        LineItem modelLine;
        LineItem trainingDataLine;
        LineItem lossDataLine;
        LineItem predictedLine;
        LineItem testDataLine;

        WordStemming ws = new WordStemming();
        WordVec wv = new WordVec();
        VectorFile vf = new VectorFile();
        public string[] loutvec;
        public string[,] outdata = new string[20, 2];
        public string[] outdata_ru;
        private string outtext = "";
        string perres = "";
        private List<string> word_ver = new List<string>();

        //Change: timestep+, batchSize+, inDim+, func+, koef, stop words+

        public LSTMTimeSeries()
        {
            InitializeComponent();
            InitiGraphs();
            //load data
            //load data in to memory
            // var xdata1 = LinSpace(0, 100.0, 10000).Select(x => (float)x).ToArray<float>();
            // var model = Model.Load("textpaint.txt");
            //var xdata = wv.vec.Select(x => (float)x).ToArray<float>();

            //buildfuncinit();
        }

        private void buildfuncinit()
        {
            func = Math.Sin;
            batchSize = 100;
            koef = 0.2f;
            buildfunc();
            loadListView(DataSet["features"].train, DataSet["label"].train);
            loadGraphs(DataSet["label"].train, DataSet["label"].test);
            outdata_ru = loutvec;
        }

        

        private void buildfunc()
        {
            float[] xdata;
            clearAll();

           if ((radioButton1.Checked == true || radioButton2.Checked == true || radioButton3.Checked == true) )
            {
                wv.wv();
                word_ver = wv.list_word_ver;
                xdata = new float[wv.vec.Length * wv.vec.Length];
                int j = 0;
                for (int i = 0; i < wv.vec.Length * wv.vec.Length; i++)
                {
                    if (i % wv.vec.Length == 0)
                        j = 0;
                    xdata[i] = wv.vec[j];
                    j++;
                }
                inDim = xdata.Length / wv.vec.Length;
                ouDim = inDim;
                timeStep = inDim;
                DataSet = loadWaveDataset(func, xdata, inDim, timeStep);
                outdata_ru = loutvec;

                listView3.Items.Add("INPUT DATA");
                string intext = wv.origintext;
                string[] intextarr = intext.Split('/', ',', '.', ':', ';', '[', ']', '{', '}', '(', ')', '-', '?', '!', ' ');
                string[] arrtemp = new string[intextarr.Length];
                for (int i = 0; i < intextarr.Length; i++)
                    for(int f = 0; f < wv.list_word_ver.Count; f++)
                {
                        if (intextarr[i].Trim() != "")
                        if (ws.Stem(intextarr[i]) == ws.Stem(wv.list_word_ver[f].Split('-')[1].ToString()))
                            arrtemp[i] = wv.list_word_ver[f].Split('-')[0].ToString();
                }
                for (int i = 0; i < arrtemp.Length; i++)
                {
                    if (arrtemp[i] == null)
                        arrtemp[i] = "0";
                    listView3.Items.Add(arrtemp[i] + " ");
                }
            }


            listView3.Items.Add( "\n\n ");
        }

        private void InitiGraphs()
        {
            ///Fitness simulation chart
            zedGraphControl4.GraphPane.Title.Text = "Model evaluation";
            zedGraphControl4.GraphPane.XAxis.Title.Text = "Samples";
            zedGraphControl4.GraphPane.YAxis.Title.Text = "Observer/Predicted";

            trainingDataLine = new LineItem("Data Points", null, null, Color.Red, ZedGraph.SymbolType.None, 1);
            trainingDataLine.Symbol.Fill = new Fill(Color.Red);
            trainingDataLine.Symbol.Size = 1;

            modelLine = new LineItem("Data Points", null, null, Color.Blue, ZedGraph.SymbolType.None, 1);
            modelLine.Symbol.Fill = new Fill(Color.Red);
            modelLine.Symbol.Size = 1;

            zedGraphControl5.GraphPane.XAxis.Title.Text = "Training Loss";
            zedGraphControl5.GraphPane.XAxis.Title.Text = "Iteration";
            zedGraphControl5.GraphPane.YAxis.Title.Text = "Loss value";

            lossDataLine = new LineItem("Loss values", null, null, Color.Red, ZedGraph.SymbolType.Circle, 1);
            lossDataLine.Symbol.Fill = new Fill(Color.Red);
            lossDataLine.Symbol.Size = 5;

            //Add line to graph
            this.zedGraphControl4.GraphPane.CurveList.Add(trainingDataLine);
            // this.zedGraphControl1.GraphPane.AxisChange(this.CreateGraphics());
            this.zedGraphControl4.GraphPane.CurveList.Add(modelLine);
            this.zedGraphControl4.GraphPane.AxisChange(this.CreateGraphics());


            this.zedGraphControl5.GraphPane.CurveList.Add(lossDataLine);
            this.zedGraphControl5.GraphPane.AxisChange(this.CreateGraphics());


            zedGraphControl6.GraphPane.Title.Text = "Model testing";
            zedGraphControl6.GraphPane.XAxis.Title.Text = "Samples";
            zedGraphControl6.GraphPane.YAxis.Title.Text = "Observer/Predicted";

            testDataLine = new LineItem("Actual Data", null, null, Color.Red, ZedGraph.SymbolType.None, 1);
            testDataLine.Symbol.Fill = new Fill(Color.Red);
            testDataLine.Symbol.Size = 1;

            predictedLine = new LineItem("Prediction", null, null, Color.Blue, ZedGraph.SymbolType.None, 1);
            predictedLine.Symbol.Fill = new Fill(Color.Red);
            predictedLine.Symbol.Size = 1;


            this.zedGraphControl6.GraphPane.CurveList.Add(testDataLine);
            this.zedGraphControl6.GraphPane.AxisChange(this.CreateGraphics());
            this.zedGraphControl6.GraphPane.CurveList.Add(predictedLine);
            this.zedGraphControl6.GraphPane.AxisChange(this.CreateGraphics());

            // zedGraphControl1.RestoreScale(zedGraphControl1.GraphPane);

            //zedGraphControl3.GraphPane.YAxis.Title.Text = "Model Testing";
        }

        private void CNTKDemo_Load(object sender, EventArgs e)
        {
           // loadListView(DataSet["features"].train, DataSet["label"].train);
           // loadGraphs(DataSet["label"].train, DataSet["label"].test);
        }

        private void loadGraphs(float[][] train, float[][] test)
        {
            for (int i = 0; i < train.Length; i++)
                trainingDataLine.AddPoint(new PointPair(i + 1, train[i][0]));

            for (int i = 0; i < test.Length; i++)
                testDataLine.AddPoint(new PointPair(i + 1, test[i][0]));

            zedGraphControl1.RestoreScale(zedGraphControl1.GraphPane);
            zedGraphControl3.RestoreScale(zedGraphControl3.GraphPane);

        }

        private void loadListView(float[][] X, float[][] Y)
        {
            loutvec = new string[X[0].Length];
            for (int j = 0; j < Y[batchSize].Length; j++)
                loutvec[j] = (Y[batchSize][j].ToString());

            // showMessageBox();
            // showresult();

            if ((radioButton1.Checked == true || radioButton2.Checked == true || radioButton3.Checked == true))
                showresultintbtfidf();
          
            listView3.Items.Add("OUTPUT DATA");
            for (int j = 0; j < loutvec.Length; j++)
                listView3.Items.Add(loutvec[j].ToString());
        }


        private void showresultintbmodel()
        {
           
            string str1 = "";
            for (int j = 0; j < loutvec.Length; j++)
            {
                if (Convert.ToDouble(loutvec[j]) > koef)
                    str1 +=vf.arr_datatosum[j] + "  ";
            }

            string str = "", strcalc = "";
            string[] strarr = str1.Split(' ');


            richTextBox1.Text = "";
          
            string[] textMass2 =vf.datatosum.Split(' ', ',', ':', ';', '-', '\n', '.', '?', '!');
            string pattern = @"\b[а-яa-z]+\b";
            Regex reg = new Regex(pattern);
            MatchCollection mc1 = reg.Matches(strcalc);
            string regtext1 = "";
            foreach (Match m in mc1)
            {
                regtext1 += m.Value + " ";
            }
            string[] textMass1 = regtext1.Split(' ');
            textBox5.Text = textMass1.Length.ToString();
            textBox6.Text = textMass2.Length.ToString();
            textBox7.Text = (100 - (double)((textMass1.Length * 100) / (textMass2.Length))).ToString();

            perres = " Word " + textMass1.Length.ToString() + " Was word " + textMass2.Length.ToString() + " % " + (100 - (double)((textMass1.Length * 100) / (textMass2.Length))).ToString();
        }

        private void showresultintbtfidf()
        {
            foreach (string el in word_ver)
                listView2.Items.Add(el + "\n");

            string str1 = "";
            for (int j = 0; j < loutvec.Length; j++)
            {
                if (Convert.ToDouble(loutvec[j]) > koef)
                    str1 += wv.list_wv[j].Split('-')[0].ToString() + "  ";
            }

            string str = "",str3 ="",strcalc="";
            string[] strarr = str1.Split(' ');
            
            
            richTextBox1.Text = "";
            if (radioButton1.Checked == true || radioButton2.Checked == true)
            {
                for (int j = 0; j < strarr.Length; j++)
                {
                    if (strarr[j] != "")
                        for (int i = 0; i < wv.list_str.Count; i++)
                            if (wv.list_str[i].Contains((strarr[j])))
                                if (!str.Contains(wv.list_str[i]))
                                    str += wv.list_str[i] + ". ";
                }
                richTextBox1.Text = str;
                outtext = str;
                strcalc = str;
            }
           
            if (radioButton3.Checked == true)
            {
                for (int j = 0; j < strarr.Length; j++)
                {
                    if (strarr[j] != "")
                    {
                        for (int i = 0; i < wv.list_str.Count / 3; i++)
                            if (wv.list_str[i].Contains((strarr[j])))
                                if (!str3.Contains(wv.list_str[i]))
                                    str3 += wv.list_str[i] + ". ";
                        for (int i = (2 * wv.list_str.Count)/3; i < wv.list_str.Count; i++)
                            if (wv.list_str[i].Contains((strarr[j])))
                                if (!str3.Contains(wv.list_str[i]))
                                    str3 += wv.list_str[i] + ". ";
                    }
                }
                richTextBox1.Text = str3;
                outtext = str3;
                strcalc = str3;
            }

            string[] textMass2 = wv.origintext.Split(' ', ',', ':', ';', '-', '\n', '.', '?', '!');
            string pattern = @"\b[а-яa-z]+\b";
            Regex reg = new Regex(pattern);
            MatchCollection mc1 = reg.Matches(strcalc);
            string regtext1 = "";
            foreach (Match m in mc1)
            {
                regtext1 += m.Value + " ";
            }
            string[] textMass1 = regtext1.Split(' ');
            textBox5.Text = textMass1.Length.ToString();
            textBox6.Text = textMass2.Length.ToString();
            textBox7.Text = (100 - (double)((textMass1.Length * 100) / (textMass2.Length))).ToString();

            perres = " Word " + textMass1.Length.ToString() + " Was word " + textMass2.Length.ToString() + " % " + (100 - (double)((textMass1.Length * 100) / (textMass2.Length))).ToString();
        }

        private void showresult()
        {
            string str1 = "";
            for (int j = 0; j < loutvec.Length; j++)
            {
                if (Convert.ToDouble(loutvec[j]) > koef)
                    str1 += wv.list_wv[j].Split('-')[0].ToString() + "  ";
            }
            MessageBox.Show(str1);

            string str = "";
            string[] strarr = str1.Split(' ');
            for (int j = 0; j < strarr.Length; j++)
            {
                if (strarr[j] != "")
                    for (int i = 0; i < wv.list_str.Count; i++)
                        if (wv.list_str[i].Contains((strarr[j])))
                            if (!str.Contains(wv.list_str[i]))
                                str += wv.list_str[i] + "  ";
            }
            MessageBox.Show(str);
            string[] textMass2 = wv.origintext.Split(' ', ',', ':', ';', '-', '\n', '.', '?', '!');
            string pattern = @"\b[а-яa-z]+\b";
            Regex reg = new Regex(pattern);
            MatchCollection mc1 = reg.Matches(str);
            string regtext1 = "";
            foreach (Match m in mc1)
            {
                regtext1 += m.Value + " ";
            }
            string[] textMass1 = regtext1.Split(' ');
            textBox5.Text = textMass1.Length.ToString();
            textBox6.Text = textMass2.Length.ToString();
            textBox7.Text = (100 - (double)((textMass1.Length * 100) / (textMass2.Length))).ToString();
        }

        private void showMessageBox()
        {
            string str = "";
            for (int j = 0; j < loutvec.Length; j++)
                str += wv.list_wv[j] + " " + loutvec[j].ToString() + "\t";
            MessageBox.Show(str);

            str = "";
            for (int j = 0; j < loutvec.Length; j++)
                str += wv.list_wv[j].Split('-')[0].ToString() + " " + loutvec[j].ToString() + "\t";
            MessageBox.Show(str);

            str = "";
            for (int j = 0; j < loutvec.Length; j++)
            {
                str += wv.list_wv[j].Split('-')[0].ToString() + "  ";
            }
            MessageBox.Show(str);
        }


        /// <summary>
        /// Iteration method for enumerating data during iteration process of training
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="mMSize"></param>
        /// <returns></returns>
        private static IEnumerable<(float[] X, float[] Y)> nextBatch(float[][] X, float[][] Y, int mMSize)
        {

            float[] asBatch(float[][] data, int start, int count)
            {
                var lst = new List<float>();
                for (int i = start; i < start + count; i++)
                {
                    if (i >= data.Length)
                        break;

                    lst.AddRange(data[i]);
                }
                return lst.ToArray();
            }

            for (int i = 0; i <= X.Length - 1; i += mMSize)
            {
                var size = X.Length - i;
                if (size > 0 && size > mMSize)
                    size = mMSize;

                var x = asBatch(X, i, size);
                var y = asBatch(Y, i, size);

                yield return (x, y);
            }

        }
        /// <summary>
        /// Method of generating wave function y=sin(x)
        /// </summary>
        /// <param name="fun"></param>
        /// <param name="x0"></param>
        /// <param name="timeSteps"></param>
        /// <param name="timeShift"></param>
        /// <returns></returns>
        static Dictionary<string, (float[][] train, float[][] valid, float[][] test)> loadWaveDataset(Func<double, double> fun, float[] x0, int timeSteps, int timeShift)
        {
            ////fill data
            float[] xsin = new float[x0.Length];//all data
            for (int l = 0; l < x0.Length; l++)
                xsin[l] = (float)fun(x0[l]);


            //split data on training and testing part
            var a = new float[xsin.Length - timeShift];
            var b = new float[xsin.Length - timeShift];

            for (int l = 0; l < xsin.Length; l++)
            {
                //
                if (l < xsin.Length - timeShift)
                    a[l] = xsin[l];

                //
                if (l >= timeShift)
                    b[l - timeShift] = xsin[l];
            }

            //make arrays of data
            var a1 = new List<float[]>();
            var b1 = new List<float[]>();
            for (int i = 0; i < a.Length - timeSteps + 1; i++)
            {
                //features
                var row = new float[timeSteps];
                for (int j = 0; j < timeSteps; j++)
                    row[j] = a[i + j];
                //create features row
                a1.Add(row);
                //label row
                //b1.Add(new float[] { b[i + timeSteps - 1] });

                var rowb = new float[timeSteps];
                for (int j = 0; j < timeSteps; j++)
                    rowb[j] = b[i + j];
                //create features row
                b1.Add(rowb);
            }

            //split data into train, validation and test data set
            var xxx = splitData(a1.ToArray(), 0.1f, 0.1f);
            var yyy = splitData(b1.ToArray(), 0.1f, 0.1f);


            var retVal = new Dictionary<string, (float[][] train, float[][] valid, float[][] test)>();
            retVal.Add("features", xxx);
            retVal.Add("label", yyy);
            return retVal;
        }

        /// <summary>
        /// Split data on training validation and testing data sets
        /// </summary>
        /// <param name="data">full data </param>
        /// <param name="valSize">percentage amount of validation </param>
        /// <param name="testSize">percentage amount for testing</param>
        /// <returns></returns>
        static (float[][] train, float[][] valid, float[][] test) splitData(float[][] data, float valSize = 0.1f, float testSize = 0.1f)
        {
            //calculate
            var posTest = (int)(data.Length * (1 - testSize));
            var posVal = (int)(posTest * (1 - valSize));

            return (data.Skip(0).Take(posVal).ToArray(), data.Skip(posVal).Take(posTest - posVal).ToArray(), data.Skip(posTest).ToArray());
        }

        /// <summary>
        /// Taken from https://gist.github.com/wcharczuk/3948606
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="num"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public static IEnumerable<double> LinSpace(double start, double stop, int num, bool endpoint = true)
        {
            var result = new List<double>();
            if (num <= 0)
            {
                return result;
            }

            if (endpoint)
            {
                if (num == 1)
                {
                    return new List<double>() { start };
                }

                var step = (stop - start) / ((double)num - 1.0d);
                result = Arange(0, num).Select(v => (v * step) + start).ToList();
            }
            else
            {
                var step = (stop - start) / (double)num;
                result = Arange(0, num).Select(v => (v * step) + start).ToList();
            }

            return result;
        }
        /// <summary>
        /// Taken from https://gist.github.com/wcharczuk/3948606
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<double> Arange(double start, int count)
        {
            return Enumerable.Range((int)start, count).Select(v => (double)v);
        }

        
        private void train(Dictionary<string, (float[][] train, float[][] valid, float[][] test)> dataSet, 
            int hiDim, int cellDim, int iteration, int batchSize, Action<Trainer, Function, int, DeviceDescriptor> progressReport, DeviceDescriptor device)
        {
            //split dataset on train, validate and test parts
            var featureSet = dataSet["features"];
            var labelSet = dataSet["label"];

            // build the model
            var feature = Variable.InputVariable(new int[] { inDim }, DataType.Float, featuresName, null, false /*isSparse*/);
            var label = Variable.InputVariable(new int[] { ouDim }, DataType.Float, labelsName, new List<CNTK.Axis>() { CNTK.Axis.DefaultBatchAxis() }, false);

            var lstmModel = LSTMHelper.CreateModel(feature, ouDim, hiDim, cellDim, device, "timeSeriesOutput");

            Function trainingLoss = CNTKLib.SquaredError(lstmModel, label, "squarederrorLoss");
            Function prediction = CNTKLib.SquaredError(lstmModel, label, "squarederrorEval");


            // prepare for training
            TrainingParameterScheduleDouble learningRatePerSample = new TrainingParameterScheduleDouble(0.0005, 1);
            TrainingParameterScheduleDouble momentumTimeConstant = CNTKLib.MomentumAsTimeConstantSchedule(256);

            IList<Learner> parameterLearners = new List<Learner>() {
                Learner.MomentumSGDLearner(lstmModel.Parameters(), learningRatePerSample, momentumTimeConstant, /*unitGainMomentum = */true)  };

            //create trainer
            var trainer = Trainer.CreateTrainer(lstmModel, trainingLoss, prediction, parameterLearners);

            // train the model
            for (int i = 1; i <= iteration; i++)
            {
                //get the next minibatch amount of data
                foreach (var miniBatchData in nextBatch(featureSet.train, labelSet.train, batchSize))
                {
                    var xValues = Value.CreateBatch<float>(new NDShape(1, inDim), miniBatchData.X, device);
                    var yValues = Value.CreateBatch<float>(new NDShape(1, ouDim), miniBatchData.Y, device);

                    //Combine variables and data in to Dictionary for the training
                    var batchData = new Dictionary<Variable, Value>();
                    batchData.Add(feature, xValues);
                    batchData.Add(label, yValues);

                    //train minibarch data
                    trainer.TrainMinibatch(batchData, device);
                }

                if (this.InvokeRequired)
                {
                    // Execute the same method, but this time on the GUI thread
                    this.Invoke(
                        new Action(() =>
                        {
                            //output training process
                            progressReport(trainer, lstmModel.Clone(), i, device);
                        }
                        ));
                }
                else
                {
                    //output training process
                    progressReport(trainer, lstmModel.Clone(), i, device);

                }             
            }
        }

        

        private void reportOnGraphs(Trainer trainer, Function model, int i, DeviceDescriptor device)
        {
            currentModelEvaluation(trainer, model, i, device);
            currentModelTest(trainer, model, i, device);
        }

        private void currentModelTest(Trainer trainer, Function model, int i, DeviceDescriptor device)
        {
            //get the next minibatch amount of data
            int sample = 1;
            predictedLine.Clear();
            foreach (var miniBatchData in nextBatch(DataSet["features"].test, DataSet["label"].test, batchSize))
            {
                //get data from dataset
                var xValues = Value.CreateBatch<float>(new NDShape(1, inDim), miniBatchData.X, device);
                var yValues = Value.CreateBatch<float>(new NDShape(1, ouDim), miniBatchData.Y, device);

                //model evaluation
                var fea = model.Arguments[0];
                var lab = model.Output;
                //evaluation preparation
                var inputDataMap = new Dictionary<Variable, Value>() { { fea, xValues } };
                var outputDataMap = new Dictionary<Variable, Value>() { { lab, null } };
                model.Evaluate(inputDataMap, outputDataMap, device);
                //extract the data
                var oData = outputDataMap[lab].GetDenseData<float>(lab);
                //show on graph
                foreach (var y in oData)
                    predictedLine.AddPoint(new PointPair(sample++, y[0]));
            }
            zedGraphControl3.RestoreScale(zedGraphControl3.GraphPane);
        }

        private void currentModelEvaluation(Trainer trainer, Function model, int i, DeviceDescriptor device)
        {
            lossDataLine.AddPoint(new PointPair(i, trainer.PreviousMinibatchLossAverage()));

            //get the next minibatch amount of data
            int sample = 1;
            modelLine.Clear();
            foreach (var miniBatchData in nextBatch(DataSet["features"].train, DataSet["label"].train, batchSize))
            {
                var xValues = Value.CreateBatch<float>(new NDShape(1, inDim), miniBatchData.X, device);
                var yValues = Value.CreateBatch<float>(new NDShape(1, ouDim), miniBatchData.Y, device);

                //model evaluation
                // build the model

                // var fea = Variable.InputVariable(new int[] { inDim }, DataType.Float, featuresName, null, false /*isSparse*/);
                // var lab = Variable.InputVariable(new int[] { ouDim }, DataType.Float, labelsName, new List<CNTK.Axis>() { CNTK.Axis.DefaultBatchAxis() }, false);
                var fea = model.Arguments[0];
                var lab = model.Output;

                var inputDataMap = new Dictionary<Variable, Value>() { { fea, xValues } };
                var outputDataMap = new Dictionary<Variable, Value>() { { lab, null } };
                model.Evaluate(inputDataMap, outputDataMap, device);

                var oData = outputDataMap[lab].GetDenseData<float>(lab);

                foreach (var y in oData)
                    modelLine.AddPoint(new PointPair(sample++, y[0]));
            }
            zedGraphControl1.RestoreScale(zedGraphControl1.GraphPane);
            zedGraphControl2.RestoreScale(zedGraphControl2.GraphPane);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OriginTextForm otf = new OriginTextForm();
            if ((radioButton1.Checked == true || radioButton2.Checked == true || radioButton3.Checked == true))
                otf.sendData(wv.origintext);      
            otf.Show();
        }

        private void getdatakoef()
        {
            string str1 = "";
            string str = "";
            string[] textMass2 = { "" };
            if ((radioButton1.Checked == true || radioButton2.Checked == true || radioButton3.Checked == true))
            {
                for (int j = 0; j < loutvec.Length; j++)
                {
                    if (Convert.ToDouble(loutvec[j]) > koef)
                        str1 += wv.list_wv[j].Split('-')[0].ToString() + "  ";
                }

                string[] strarr = str1.Split(' ');
                for (int j = 0; j < strarr.Length; j++)
                {
                    if (strarr[j] != "")
                        for (int i = 0; i < wv.list_str.Count; i++)
                            if (wv.list_str[i].Contains((strarr[j])))
                                if (!str.Contains(wv.list_str[i]))
                                    str += wv.list_str[i] + "  ";
                }

                richTextBox1.Text = str;
                outtext = str;

                textMass2 = wv.origintext.Split(' ', ',', ':', ';', '-', '\n', '.', '?', '!');
            }

            

            string pattern = @"\b[а-яa-z]+\b";
            Regex reg = new Regex(pattern);
            MatchCollection mc1 = reg.Matches(str);
            string regtext1 = "";
            foreach (Match m in mc1)
            {
                regtext1 += m.Value + " ";
            }
            string[] textMass1 = regtext1.Split(' ');

            perres =  " Word " + textMass1.Length.ToString() + " Was word " + textMass2.Length.ToString() + " % " + (100 - (double)((textMass1.Length * 100) / (textMass2.Length))).ToString() ;
        }

        private void buildfunc1Sin()
        {
            func = Math.Sin;
            batchSize = 50;
            koef = 0.1f;
            outdata[0, 0] = "BatchSize " + batchSize + " K " + koef;
            buildfunc();
            loadListView(DataSet["features"].train, DataSet["label"].train);
            outdata[0, 1] = outtext;
            outdata[0, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 50;
            koef = 0.2f;
            outdata[1, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[1, 1] = outtext;
            outdata[1, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 50;
            koef = 0.25f;
            outdata[2, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[2, 1] = outtext;
            outdata[2, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 50;
            koef = 0.3f;
            outdata[3, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[3, 1] = outtext;
            outdata[3, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 50;
            koef = 0.5f;
            outdata[4, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[4, 1] = outtext;
            outdata[4, 0] += "\nOut info" + perres;


            func = Math.Sin;
            batchSize = 100;
            koef = 0.1f;
            outdata[5, 0] = "BatchSize " + batchSize + " K " + koef;
            buildfunc();
            loadListView(DataSet["features"].train, DataSet["label"].train);
            outdata[5, 1] = outtext;
            outdata[5, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 100;
            koef = 0.2f;
            outdata[6, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[6, 1] = outtext;
            outdata[6, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 50;
            koef = 0.25f;
            outdata[7, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[7, 1] = outtext;
            outdata[7, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 100;
            koef = 0.3f;
            outdata[8, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[8, 1] = outtext;
            outdata[8, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 100;
            koef = 0.5f;
            outdata[9, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[9, 1] = outtext;
            outdata[9, 0] += "\nOut info" + perres;


            func = Math.Sin;
            batchSize = 250;
            koef = 0.1f;
            outdata[10, 0] = "BatchSize " + batchSize + " K " + koef; ;
            buildfunc();
            loadListView(DataSet["features"].train, DataSet["label"].train);
            outdata[10, 1] = outtext;
            outdata[10, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 250;
            koef = 0.2f;
            outdata[11, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[11, 1] = outtext;
            outdata[11, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 250;
            koef = 0.25f;
            outdata[12, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[12, 1] = outtext;
            outdata[12, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 250;
            koef = 0.3f;
            outdata[13, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[13, 1] = outtext;
            outdata[13, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 250;
            koef = 0.5f;
            outdata[14, 0] = "BatchSize " + batchSize + " K " + koef; ;
            getdatakoef();
            outdata[14, 1] = outtext;
            outdata[14, 0] += "\nOut info" + perres;


            func = Math.Sin;
            batchSize = 500;
            koef = 0.1f;
            outdata[15, 0] = "BatchSize " + batchSize + " K " + koef;
            buildfunc();
            loadListView(DataSet["features"].train, DataSet["label"].train);
            outdata[15, 1] = outtext;
            outdata[15, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 500;
            koef = 0.2f;
            outdata[16, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[16, 1] = outtext;
            outdata[16, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 500;
            koef = 0.25f;
            outdata[17, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[17, 1] = outtext;
            outdata[17, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 500;
            koef = 0.3f;
            outdata[18, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[18, 1] = outtext;
            outdata[18, 0] += "\nOut info" + perres;

            func = Math.Sin;
            batchSize = 500;
            koef = 0.5f;
            outdata[19, 0] = "BatchSize " + batchSize + " K " + koef;
            getdatakoef();
            outdata[19, 1] = outtext;
            outdata[19, 0] += "\nOut info" + perres;

        }



        private void clearAll()
        {
            listView2.Clear();
            listView3.Clear();
            modelLine.Clear();
            trainingDataLine.Clear();
            lossDataLine.Clear();
            predictedLine.Clear();
            testDataLine.Clear();
            DataSet = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
             buildfunc1Sin();
            ShowResultForm srf = new ShowResultForm();
            if ((radioButton1.Checked == true || radioButton2.Checked == true || radioButton3.Checked == true))
                srf.sendData(wv.origintext);
            srf.sendData_arr(outdata);
            srf.Show();
            new LSTMTimeSeries();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView2.Clear();
            batchSize = Convert.ToInt32(textBox1.Text);
            double ss = 0;
            Double.TryParse(comboBox1.SelectedItem.ToString().Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out ss);
            koef = (float)ss;
            if (check() == true)
            {
                buildfunc();
                loadListView(DataSet["features"].train, DataSet["label"].train);
                loadGraphs(DataSet["label"].train, DataSet["label"].test);
            }
            else { MessageBox.Show("Check input files. English texts - Model. Russian texts - TFIDF"); }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Select_text stf = new Select_text();
            stf.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            RelativeUtility ruf = new RelativeUtility();
            if((radioButton1.Checked == true || radioButton2.Checked == true || radioButton3.Checked == true))
                 ruf.sendData(wv.origintext);          
            ruf.sendData_arr(outtext.Split('.'));
            ruf.Show();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            string fn = Select_text.ReadFN();
            if (!File.Exists(fn))
                throw new FileNotFoundException();
            string data = File.ReadAllText(fn, Encoding.Default);
            File.WriteAllText(fn, new_text(data), Encoding.Default);
        }
        private string new_text(string data)
        {
            string ret_str = "";
            string[] split_data = data.Trim().Split('.', '?', '!');
            for (int i = 0; i < split_data.Length/3; i++)
            {
                if (split_data[i].Trim() != "")
                    ret_str += split_data[i] + ".";
            }

            for (int j = 2 * split_data.Length / 3; j < split_data.Length; j++)
             {
                if (split_data[j].Trim() != "")
                    ret_str += split_data[j] + ".";
            }
            return ret_str;
        }

        private bool check()
        {
            bool flag = false;
            if ((radioButton1.Checked == true || radioButton2.Checked == true || radioButton3.Checked == true) && (Select_text.ReadFN() != "textwordembedding.txt"))
            {
                flag = true;
            }
           
            return flag;
        }
    }
}
