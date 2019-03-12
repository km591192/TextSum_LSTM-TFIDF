using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTMTimeSeriesDemo
{
    class Call_print
    {
        Realize_print NN = new Realize_print();

        public List<string> printword()
        {
            return NN.printword();
        }

        public List<string> printstr()
        {
            return NN.printstr();
        }

        public List<string> printwordsw()
        {
            return NN.printwordsw();
        }

        public String printorigintext()
        {
            return NN.printorigintext();
        }

        public List<string> printword_count()
        {
            return NN.printword_count();
        }

        public List<string> printword_ver_TFIDF()
        {
            return NN.printword_ver_TFIDF();
        }
        public List<string> printword_ver_TFSLF()
        {
            return NN.printword_ver_TFSLF();
        }

        public List<string> print_highverword()
        {
            return NN.print_highverword();
        }

        public List<string> print_highwordcount()
        {
            return NN.print_highwordcount();
        }
    }
}
