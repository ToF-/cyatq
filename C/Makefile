cyatq : cyatq.c
	cc cyatq.c -o cyatq

test : cyatq test.txt
	sed -n -e 's/\(<- \)\(.*\)/\2/pw input.dat'     test.txt >/dev/null
	sed -n -e 's/\(-> \)\(.*\)/\2/pw expected.dat' test.txt >/dev/null
	./cyatq <input.dat >result.dat
	diff expected.dat result.dat

random : cyatq
	./maketestdata 100 50000 >test.txt
	sed -n -e 's/\(<- \)\(.*\)/\2/pw input.dat'     test.txt >/dev/null
	sed -n -e 's/\(-> \)\(.*\)/\2/pw expected.dat' test.txt >/dev/null
	./cyatq <input.dat >result.dat
	diff expected.dat result.dat


clean : 
	rm cyatq;rm *.dat; rm maketestdata

data :
	cc maketestdata.c -o maketestdata
	

