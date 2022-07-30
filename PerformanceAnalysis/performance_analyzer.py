import sys
import matplotlib.pyplot as plt



def extract_info(line):
	stats_s = line.split('[')[1].split(']')[0].split(',')
	stats = dict()
	for stat in stats_s:
		key = stat.split(':')[0]
		value = int(stat.split(':')[1])
		stats[key] = value
	return stats


def get_cmap(n, name='hsv'):
    '''Returns a function that maps each index in 0, 1, ..., n-1 to a distinct 
    RGB color; the keyword argument name must be a standard mpl colormap name.'''
    return plt.cm.get_cmap(name, n)



def main():
	args = sys.argv[1:]
	if len(args) == 0:
		log_file = '../client/Assets/StreamingAssets/log.txt'

	else:
		log_file = args[0]

	stats = []
	with open(log_file) as f:
		for line in f.read().splitlines():
			if 'PerformanceTracker' in line:
				stats.append(extract_info(line))

	keys = list(stats[0].keys())
	keys.remove('Seconds')
	seconds = [stat['Seconds'] for stat in stats]

	cmap = get_cmap(len(keys) * 5)
	for i, key in enumerate(keys):
		values = [stat[key] for stat in stats]
		plt.plot(seconds, values, c=cmap(i*5), label=key)

	plt.legend(loc="upper left")
	plt.show()


if __name__ == "__main__":
	main()
