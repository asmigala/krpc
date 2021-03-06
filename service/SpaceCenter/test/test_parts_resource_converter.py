import unittest
import testingtools
import krpc
import time

class TestPartsResourceConverter(testingtools.TestCase):

    @classmethod
    def setUpClass(cls):
        if testingtools.connect().space_center.active_vessel.name != 'PartsHarvester':
            testingtools.new_save()
            testingtools.launch_vessel_from_vab('PartsHarvester')
            testingtools.remove_other_vessels()
        cls.conn = testingtools.connect(name='TestPartsResourceConverter')
        cls.vessel = cls.conn.space_center.active_vessel
        cls.parts = cls.vessel.parts
        cls.state = cls.conn.space_center.ResourceConverterState
        cls.drill = next(iter(filter(lambda e: e.part.title == '\'Drill-O-Matic\' Mining Excavator', cls.parts.resource_harvesters)))
        cls.converter = next(iter(filter(lambda e: e.part.title == 'Convert-O-Tron 250', cls.parts.resource_converters)))
        cls.infos = [
            {
                'name': 'Lf+Ox',
                'inputs': ['Ore', 'ElectricCharge'],
                'outputs': ['LiquidFuel', 'Oxidizer']
            },
            {
                'name': 'Monoprop',
                'inputs': ['Ore', 'ElectricCharge'],
                'outputs': ['MonoPropellant']
            },
            {
                'name': 'LiquidFuel',
                'inputs': ['Ore', 'ElectricCharge'],
                'outputs': ['LiquidFuel']
            },
            {
                'name': 'Oxidizer',
                'inputs': ['Ore', 'ElectricCharge'],
                'outputs': ['Oxidizer']
            }
        ]

    @classmethod
    def tearDownClass(cls):
        cls.conn.close()

    def test_properties(self):
        self.assertEqual(len(self.infos), self.converter.count)
        for i,info in enumerate(self.infos):
            self.assertFalse(self.converter.active(i))
            self.assertEqual(info['name'], self.converter.name(i))
            self.assertEqual(self.state.idle, self.converter.state(i))
            self.assertEqual('Inactive', self.converter.status_info(i))
            self.assertEqual(info['inputs'], self.converter.inputs(i))
            self.assertEqual(info['outputs'], self.converter.outputs(i))

    def test_operate(self):
        self.drill.deployed = True
        while not self.drill.deployed:
            time.sleep(0.1)
        self.drill.active = True
        while not self.drill.active:
            time.sleep(0.1)
        index = 1
        self.assertFalse(self.converter.active(index))
        self.assertEqual(self.state.idle, self.converter.state(index))
        self.assertEqual('Inactive', self.converter.status_info(index))
        self.converter.start(index)
        time.sleep(0.1)
        self.assertTrue(self.converter.active(index))
        self.assertEqual(self.state.running, self.converter.state(index))
        self.converter.stop(index)
        time.sleep(0.1)
        self.assertFalse(self.converter.active(index))
        self.assertEqual(self.state.idle, self.converter.state(index))
        self.assertEqual('Inactive', self.converter.status_info(index))
        self.drill.deployed = False
        while self.drill.state != self.conn.space_center.ResourceHarvesterState.retracted:
            time.sleep(0.1)

if __name__ == "__main__":
    unittest.main()
