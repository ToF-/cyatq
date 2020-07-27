cyatq : cyatq.c
	cc cyatq.c -o cyatq

test : cyatq tests.txt
	sed -n -e 's/\(< \)\(.*\)/\2/pw test.dat'     tests.txt >/dev/null
	sed -n -e 's/\(> \)\(.*\)/\2/pw expected.dat' tests.txt >/dev/null
	./cyatq <test.dat >result.dat
	diff expected.dat result.dat

clean : 
	rm cyatq;rm *.dat
	
